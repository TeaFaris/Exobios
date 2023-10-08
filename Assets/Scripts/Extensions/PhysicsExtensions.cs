using UnityEngine;

public static class PhysicsExtensions
{
    public const float HU2M = 52.4934383202f;
    public const float SurfSlope = 0.7f;
    private const int MaxCollisions = 128;
    private const int MaxClipPlanes = 5;
    private const int NumBumps = 1;

    private readonly static Collider[] Colliders = new Collider[MaxCollisions];
    private readonly static Vector3[] Planes = new Vector3[MaxClipPlanes];

    public static void ResolveCollisions(CapsuleCollider capc, ref Vector3 origin, ref Vector3 velocity)
    {
        // manual collision resolving
        int numOverlaps = 0;

        GetCapsulePoints(capc, origin, out Vector3 point1, out Vector3 point2);

        numOverlaps = Physics.OverlapCapsuleNonAlloc(
                point1,
                point2,
                capc.radius,
                Colliders,
                LayerMask.GetMask("Default", "PW_Object_Small", "PW_Object_Medium", "PW_Object_Large")
            );

        for (int i = 0; i < numOverlaps; i++)
        {
            if (Colliders[i] == capc)
            {
                continue;
            }

            bool penetrated = Physics.ComputePenetration(
                                    capc,
                                    origin,
                                    Quaternion.identity,
                                    Colliders[i],
                                    Colliders[i].transform.position,
                                    Colliders[i].transform.rotation,
                                    out Vector3 direction,
                                    out float distance
                                );

            if (penetrated)
            {
                direction.Normalize();
                Vector3 penetrationVector = direction * distance;
                Vector3 velocityProjected = Vector3.Project(velocity, -direction);
                velocityProjected.y = 0; // don't touch y velocity, we need it to calculate fall damage elsewhere
                origin += penetrationVector;
                velocity -= velocityProjected;
            }
        }
    }

    public static void Friction(ref Vector3 velocity, float stopSpeed, float friction, float deltaTime)
    {
        var speed = velocity.magnitude;

        if (speed < 0.0001905f)
        {
            return;
        }

        var drop = 0f;

        // apply ground friction
        var control = (speed < stopSpeed) ? stopSpeed : speed;
        drop += control * friction * deltaTime;

        // scale the velocity
        var newSpeed = speed - drop;
        if (newSpeed < 0)
        {
            newSpeed = 0;
        }

        if (newSpeed != speed)
        {
            newSpeed /= speed;
            velocity *= newSpeed;
        }
    }

    /// <summary>
    /// Accelerate, if using for air, use <see cref="AirAccelerate"/>.
    /// </summary>
    /// <param name="currentVelocity"></param>
    /// <param name="wishDir"></param>
    /// <param name="wishSpeed"></param>
    /// <param name="accel"></param>
    /// <param name="deltaTime"></param>
    /// <param name="surfaceFriction"></param>
    /// <returns></returns>
    public static Vector3 Accelerate(Vector3 currentVelocity, Vector3 wishDir, float wishSpeed, float accel, float deltaTime, float surfaceFriction)
    {
        // See if we are changing direction a bit
        var currentSpeed = Vector3.Dot(currentVelocity, wishDir);

        // Reduce wishspeed by the amount of veer.
        var addSpeed = wishSpeed - currentSpeed;

        // If not going to add any speed, done.
        if (addSpeed <= 0)
        {
            return Vector3.zero;
        }

        // Determine amount of accleration.
        var accelSpeed = accel * deltaTime * wishSpeed * surfaceFriction;

        // Cap at addspeed
        if (accelSpeed > addSpeed)
        {
            accelSpeed = addSpeed;
        }

        var result = Vector3.zero;

        // Adjust velocity.
        for (int i = 0; i < 3; i++)
        {
            result[i] += accelSpeed * wishDir[i];
        }

        return result;
    }

    public static Vector3 AirAccelerate(Vector3 velocity, Vector3 wishDir, float wishSpeed, float accel, float airCap, float deltaTime)
    {
        var wishSpd = wishSpeed;

        wishSpd = Mathf.Min(wishSpd, airCap);

        var currentSpeed = Vector3.Dot(velocity, wishDir);

        var addSpeed = wishSpd - currentSpeed;

        if (addSpeed <= 0)
        {
            return Vector3.zero;
        }

        var accelSpeed = accel * wishSpeed * deltaTime;
        
        accelSpeed = Mathf.Min(accelSpeed, addSpeed);

        var result = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            result[i] += accelSpeed * wishDir[i];
        }

        return result;
    }

    public static int Reflect(ref Vector3 velocity, Collider collider, Vector3 origin, float deltaTime)
    {
        float d;
        var newVelocity = Vector3.zero;
        var blocked = 0;           // Assume not blocked
        var numPlanes = 0;           //  and not sliding along any planes
        var originalVelocity = velocity;  // Store original velocity
        var primalVelocity = velocity;

        var allFraction = 0f;
        var timeLeft = deltaTime;   // Total time for this movement operation.

        for (int bumpCount = 0; bumpCount < NumBumps; bumpCount++)
        {
            if (velocity.magnitude == 0f)
            {
                break;
            }

            // Assume we can move all the way from the current origin to the
            //  end point.
            var end = VectorExtensions.VectorMa(origin, timeLeft, velocity);
            var trace = TraceExtensions.TraceCollider(collider, origin, end);

            allFraction += trace.Fraction;

            if (trace.Fraction > 0)
            {
                // actually covered some distance
                originalVelocity = velocity;
                numPlanes = 0;
            }

            // If we covered the entire distance, we are done
            //  and can return.
            if (trace.Fraction == 1)
                break;      // moved the entire distance

            // If the plane we hit has a high z component in the normal, then
            //  it's probably a floor
            if (trace.PlaneNormal.y > SurfSlope)
            {
                blocked |= 1;       // floor
            }
            // If the plane has a zero z component in the normal, then it's a 
            //  step or wall
            if (trace.PlaneNormal.y == 0)
            {
                blocked |= 2;       // step / wall
            }

            // Reduce amount of m_flFrameTime left by total time left * fraction
            //  that we covered.
            timeLeft -= timeLeft * trace.Fraction;

            // Did we run out of planes to clip against?
            if (numPlanes >= MaxClipPlanes)
            {
                // this shouldn't really happen
                //  Stop our movement if so.
                velocity = Vector3.zero;
                break;
            }

            // Set up next clipping plane
            Planes[numPlanes] = trace.PlaneNormal;
            numPlanes++;

            // modify original_velocity so it parallels all of the clip planes
            //

            // reflect player velocity 
            // Only give this a try for first impact plane because you can get yourself stuck in an acute corner by jumping in place
            //  and pressing forward and nobody was really using this bounce/reflection feature anyway...
            if (numPlanes == 1)
            {
                for (int i = 0; i < numPlanes; i++)
                {
                    if (Planes[i][1] > SurfSlope)
                    {
                        // floor or slope
                        return blocked;
                        //ClipVelocity(originalVelocity, _planes[i], ref newVelocity, 1f);
                        //originalVelocity = newVelocity;
                    }
                    else
                    {
                        ClipVelocity(originalVelocity, Planes[i], ref newVelocity, 1f);
                    }
                }
                velocity = newVelocity;
                originalVelocity = newVelocity;
            }
            else
            {
                int i;
                for (i = 0; i < numPlanes; i++)
                {
                    ClipVelocity(originalVelocity, Planes[i], ref velocity, 1);

                    int j;
                    for (j = 0; j < numPlanes; j++)
                    {
                        if (j != i)
                        {
                            // Are we now moving against this plane?
                            if (Vector3.Dot(velocity, Planes[j]) < 0)
                                break;
                        }
                    }

                    if (j == numPlanes)  // Didn't have to clip, so we're ok
                        break;
                }

                // Did we go all the way through plane set
                if (i == numPlanes)
                {   // go along the crease
                    if (numPlanes != 2)
                    {
                        velocity = Vector3.zero;
                        break;
                    }
                    var dir = Vector3.Cross(Planes[0], Planes[1]).normalized;
                    d = Vector3.Dot(dir, velocity);
                    velocity = dir * d;
                }

                //
                // if original velocity is against the original velocity, stop dead
                // to avoid tiny occilations in sloping corners
                //
                d = Vector3.Dot(velocity, primalVelocity);
                if (d <= 0f)
                {
                    //Con_DPrintf("Back\n");
                    velocity = Vector3.zero;
                    break;
                }
            }
        }

        if (allFraction == 0f)
        {
            velocity = Vector3.zero;
        }

        // Check if they slammed into a wall
        //float fSlamVol = 0.0f;

        //var primal2dLen = new Vector2(primal_velocity.x, primal_velocity.z).magnitude;
        //var vel2dLen = new Vector2(_moveData.Velocity.x, _moveData.Velocity.z).magnitude;
        //float fLateralStoppingAmount = primal2dLen - vel2dLen;
        //if (fLateralStoppingAmount > PLAYER_MAX_SAFE_FALL_SPEED * 2.0f)
        //{
        //    fSlamVol = 1.0f;
        //}
        //else if (fLateralStoppingAmount > PLAYER_MAX_SAFE_FALL_SPEED)
        //{
        //    fSlamVol = 0.85f;
        //}

        //PlayerRoughLandingEffects(fSlamVol);

        return blocked;
    }

    public static int ClipVelocity(Vector3 input, Vector3 normal, ref Vector3 output, float overbounce)
    {
        var Angle = normal[1];
        var Blocked = 0x00;         // Assume unblocked.
        if (Angle > 0)          // If the plane that is blocking us has a positive z component, then assume it's a floor.
            Blocked |= 0x01;    // 
        if (Angle == 0)             // If the plane has no Z, it is vertical (wall/step)
            Blocked |= 0x02;    // 

        // Determine how far along plane to slide based on incoming direction.
        var backoff = Vector3.Dot(input, normal) * overbounce;

        for (int i = 0; i < 3; i++)
        {
            var change = normal[i] * backoff;
            output[i] = input[i] - change;
        }

        // iterate once to make sure we aren't still moving through the plane
        float adjust = Vector3.Dot(output, normal);
        if (adjust < 0.0f)
        {
            output -= (normal * adjust);
        }

        // Return blocking flags.
        return Blocked;
    }

    public static void GetCapsulePoints(CapsuleCollider capc, Vector3 origin, out Vector3 point1, out Vector3 point2)
    {
        var distanceToPoints = (capc.height / 2f) - capc.radius;
        point1 = origin + capc.center + (Vector3.up * distanceToPoints);
        point2 = origin + capc.center - (Vector3.up * distanceToPoints);
    }
}