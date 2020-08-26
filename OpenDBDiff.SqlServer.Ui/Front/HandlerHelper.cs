namespace OpenDBDiff.SqlServer.Ui
{
    public static class HandlerHelper
    {
        public delegate void SaveFilterHandler();
        public static event SaveFilterHandler OnChange;

        public static void RaiseOnChange()
        {
            OnChange?.Invoke();
        }
    }
}
