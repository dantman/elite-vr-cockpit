using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    public class OpenVR_Utils {
        public static ETextureType textureType
        {
            get
            {
                switch (SystemInfo.graphicsDeviceType)
                {
#if (UNITY_5_4)
                case UnityEngine.Rendering.GraphicsDeviceType.OpenGL2:
#endif
                    case UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore:
                    case UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2:
                    case UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3:
                        return ETextureType.OpenGL;
#if !(UNITY_5_4)
                    case UnityEngine.Rendering.GraphicsDeviceType.Vulkan:
                        return ETextureType.Vulkan;
#endif
                    default:
                        return ETextureType.DirectX;
                }

            }
        }
    }
}
