using UnityEngine;
using EasyUI.Helpers;

/* -------------------------------
   Created by : Hamza Herbou
   hamza95herbou@gmail.com
---------------------------------- */

namespace EasyUI.Toast
{

    public enum ToastColor
    {
        Black,
        Red,
        Purple,
        Magenta,
        Blue,
        Green,
        Yellow,
        Orange
    }

    public enum ToastPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public static class Toast
    {
        public static bool isLoaded = false;

        private static ToastUI toastUI;

        private static void Prepare()
        {
            if (!isLoaded)
            {
                GameObject instance = MonoBehaviour.Instantiate(Resources.Load<GameObject>("ToastUI"));
                instance.name = "[ TOAST UI ]";
                toastUI = instance.GetComponent<ToastUI>();
                isLoaded = true;
            }
        }



        public static void Show(string text, string type = "success")
        {
            Prepare();
            toastUI.Init(text, 2F, ToastColor.Black, ToastPosition.TopCenter, type);
        }


        public static void Show(string text, float duration, string type = "success")
        {
            Prepare();
            toastUI.Init(text, duration, ToastColor.Black, ToastPosition.TopCenter, type);
        }

        public static void Show(string text, float duration, ToastPosition position, string type = "success")
        {
            Prepare();
            toastUI.Init(text, duration, ToastColor.Black, position, type);
        }


        public static void Show(string text, ToastColor color, string type = "success")
        {
            Prepare();
            toastUI.Init(text, 2F, color, ToastPosition.TopCenter, type);
        }

        public static void Show(string text, ToastColor color, ToastPosition position, string type = "success")
        {
            Prepare();
            toastUI.Init(text, 2F, color, position, type);
        }


        public static void Show(string text, Color color, string type = "success")
        {
            Prepare();
            toastUI.Init(text, 2F, color, ToastPosition.TopCenter, type);
        }

        public static void Show(string text, Color color, ToastPosition position, string type = "success")
        {
            Prepare();
            toastUI.Init(text, 2F, color, position, type);
        }


        public static void Show(string text, float duration, ToastColor color, string type = "success")
        {
            Prepare();
            toastUI.Init(text, duration, color, ToastPosition.TopCenter, type);
        }

        public static void Show(string text, float duration, ToastColor color, ToastPosition position, string type = "success")
        {
            Prepare();
            toastUI.Init(text, duration, color, position, type);
        }


        public static void Show(string text, float duration, Color color, string type = "success")
        {
            Prepare();
            toastUI.Init(text, duration, color, ToastPosition.TopCenter, type);
        }

        public static void Show(string text, float duration, Color color, ToastPosition position, string type = "success")
        {
            Prepare();
            toastUI.Init(text, duration, color, position, type);
        }



        public static void Dismiss()
        {
            if (isLoaded)
                toastUI.Dismiss();
        }

    }

}
