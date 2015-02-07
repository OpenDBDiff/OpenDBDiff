namespace DBDiff.Schema.SQLServer.Generates.Front
{
    public static class HandlerHelper
    {
        public delegate void SaveFilterHandler();
        public static event SaveFilterHandler OnChange;

        public static void RaiseOnChange()
        {
            if (OnChange != null) OnChange();
        }
    }
}
