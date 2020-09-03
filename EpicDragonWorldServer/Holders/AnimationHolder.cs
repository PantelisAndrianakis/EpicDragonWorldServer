/**
 * Author: Pantelis Andrianakis
 * Date: April 21st 2019
 */
public class AnimationHolder
{
    private readonly float _velocityX;
    private readonly float _velocityZ;
    private readonly bool _triggerJump;
    private readonly bool _isInWater;
    private readonly bool _isGrounded;

    public AnimationHolder(float velocityX, float velocityZ, bool triggerJump, bool isInWater, bool isGrounded)
    {
        _velocityX = velocityX;
        _velocityZ = velocityZ;
        _triggerJump = triggerJump;
        _isInWater = isInWater;
        _isGrounded = isGrounded;
    }

    public float GetVelocityX()
    {
        return _velocityX;
    }

    public float GetVelocityZ()
    {
        return _velocityZ;
    }

    public bool IsTriggerJump()
    {
        return _triggerJump;
    }

    public bool IsInWater()
    {
        return _isInWater;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }
}
