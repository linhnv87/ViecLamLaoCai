namespace UploadFileServer.Controller
{
    internal class JMessage
    {
        public string idvanban { set; get; }
        public string Title { set; get; }
        public bool? Error { set; get; }
        public object Object { set; get; }
    }
}