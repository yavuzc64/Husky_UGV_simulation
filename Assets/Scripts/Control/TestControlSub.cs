using Unity.Robotics.ROSTCPConnector;
//using RosMessageTypes.Std;
using RosMessageTypes.Geometry;// msg c# kodunun namespace ine gore yaz 
//using Vec_ctrlMsg = RosMessageTypes.UnityRoboticsDemo.Vec_ctrlMsg;
using UnityEngine;

public class TestControlSub : MonoBehaviour
{
    public ArticulationBody wheelFL;
    public ArticulationBody wheelFR;
    public ArticulationBody wheelRL;
    public ArticulationBody wheelRR;
    public float wheelRadius = 0.67f; // Husky'nin gerçek teker çapı (yaklaşık) !! A200 modeline gore !!
    public float wheelSeparation = 0.39f;
    public float maxLinearVelocity = 5f; // m/s
    public float maxAngularVelocity = 2f; // rad/s

    public float testCoeff = 1f;
    private float halfTrack => wheelSeparation / 2f;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>("/cmd_vel", OnCmdVelReceived);
    }

    void OnCmdVelReceived(TwistMsg msg)
    {
        //Debug.Log("Linear: " + msg.linear.x + ", Angular: " + msg.angular.z);

        ApplyTwist((float)msg.linear.x, (float)msg.angular.z);
        
    }
    public void ApplyTwist(float linearX, float angularZ)
    {
        float v = linearX * maxLinearVelocity * testCoeff;
        float omega = angularZ * maxAngularVelocity * testCoeff;

        float v_r = v + omega * halfTrack;
        float v_l = v - omega * halfTrack;

        // tekerlek açısal hızları (rad/s)
        float w_r = v_r / wheelRadius;
        float w_l = v_l / wheelRadius;
        Debug.Log("v_r : " + v_r);
        Debug.Log("v_l : " + v_l);

        // ArticulationBody.SetDriveTargetVelocity kullanımı (revolute için angular birim rad/s)
        // Axis olarak X/Y/Z seçimleri joint anchor orientasyonuna göre değişir.
        wheelFL.SetDriveTargetVelocity(ArticulationDriveAxis.X, w_l);
        wheelFR.SetDriveTargetVelocity(ArticulationDriveAxis.X, w_r);
        wheelRL.SetDriveTargetVelocity(ArticulationDriveAxis.X, w_l);
        wheelRR.SetDriveTargetVelocity(ArticulationDriveAxis.X, w_r);
    }
}