using Graphics.Settings;

namespace Graphics
{
    public class SSSManager
    {
        internal static SSS SSSInstance;

        // Initialize Components
        internal void Initialize()
        {
            SSSInstance = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<SSS>();
        }

        // When user enabled the option
        internal void Start()
        {
           // initialize again 
        }
        
        // When user disabled the option
        internal void Destroy()
        {
           // cleanup render texture and garbages. 
        }
    }
}