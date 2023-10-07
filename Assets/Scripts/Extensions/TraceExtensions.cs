using UnityEngine;

public static class TraceExtensions
{
    public static Trace TraceCollider(Collider collider, Vector3 origin, Vector3 end)
    {
        if (collider is BoxCollider)
        {
            return TraceBox(origin, end, collider.bounds.extents, collider.contactOffset);
        }
        else if (collider is CapsuleCollider capc)
        {
            PhysicsExtensions.GetCapsulePoints(capc, origin, out Vector3 point1, out Vector3 point2);
            return TraceCapsule(point1, point2, capc.radius, origin, end, capc.contactOffset);
        }

        throw new System.NotImplementedException("Trace missing for collider: " + collider.GetType());
    }

    public static Trace TraceCapsule(Vector3 point1, Vector3 point2, float radius, Vector3 start, Vector3 destination, float contactOffset)
    {
        var result = new Trace()
        {
            StartPos = start,
            EndPos = destination
        };

        var longSide = Mathf.Sqrt((contactOffset * contactOffset) + (contactOffset * contactOffset));
        radius *= (1f - contactOffset);
        var direction = (destination - start).normalized;
        var maxDistance = Vector3.Distance(start, destination) + longSide;

        if (Physics.CapsuleCast(
                point1: point1,
                point2: point2,
                radius: radius,
                direction: direction,
                hitInfo: out RaycastHit hit,
                maxDistance: maxDistance
            ))
        {
            result.Fraction = hit.distance / maxDistance;
            result.HitCollider = hit.collider;
            result.HitPoint = hit.point;
            result.PlaneNormal = hit.normal;
        }
        else
        {
            result.Fraction = 1;
        }

        return result;
    }

    public static Trace TraceBox(Vector3 start, Vector3 destination, Vector3 extents, float contactOffset)
    {
        var result = new Trace()
        {
            StartPos = start,
            EndPos = destination
        };

        var longSide = Mathf.Sqrt((contactOffset * contactOffset) + (contactOffset * contactOffset));
        var direction = (destination - start).normalized;
        var maxDistance = Vector3.Distance(start, destination) + longSide;
        extents *= (1f - contactOffset);

        if (Physics.BoxCast(
                center: start,
                halfExtents: extents,
                direction: direction,
                hitInfo: out RaycastHit hit,
                orientation: Quaternion.identity,
                maxDistance: maxDistance
            ))
        {
            result.Fraction = hit.distance / maxDistance;
            result.HitCollider = hit.collider;
            result.HitPoint = hit.point;
            result.PlaneNormal = hit.normal;
        }
        else
        {
            result.Fraction = 1;
        }

        return result;
    }
}