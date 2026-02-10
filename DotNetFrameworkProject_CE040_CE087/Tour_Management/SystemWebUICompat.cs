using System;

namespace System.Web.UI
{
    public class Page
    {
        public virtual bool IsPostBack { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public HttpServerUtility Server { get; set; }
        public HttpSessionState Session { get; set; }
        public HttpApplicationState Application { get; set; }
        
        protected virtual void Page_Load(object sender, EventArgs e) { }
    }
    
    namespace WebControls
    {
        public class TextBox
        {
            public string Text { get; set; }
            public string ID { get; set; }
        }
        
        public class Button
        {
            public string Text { get; set; }
            public string ID { get; set; }
        }
        
        public class Label
        {
            public string Text { get; set; }
            public string ID { get; set; }
            public bool Visible { get; set; }
        }
        
        public class DropDownList
        {
            public string SelectedValue { get; set; }
            public string Text { get; set; }
            public string ID { get; set; }
        }
        
        public class GridView
        {
            public object DataSource { get; set; }
            public string ID { get; set; }
            public void DataBind() { }
        }
        
        public class FileUpload
        {
            public bool HasFile { get; set; }
            public string FileName { get; set; }
            public string ID { get; set; }
            public void SaveAs(string path) { }
        }
        
        public class RequiredFieldValidator
        {
            public string ControlToValidate { get; set; }
            public string ErrorMessage { get; set; }
            public string ID { get; set; }
        }
        
        public class CompareValidator
        {
            public string ControlToValidate { get; set; }
            public string ControlToCompare { get; set; }
            public string ErrorMessage { get; set; }
            public string ID { get; set; }
        }
        
        public class RegularExpressionValidator
        {
            public string ControlToValidate { get; set; }
            public string ValidationExpression { get; set; }
            public string ErrorMessage { get; set; }
            public string ID { get; set; }
        }
        
        public class RangeValidator
        {
            public string ControlToValidate { get; set; }
            public string MinimumValue { get; set; }
            public string MaximumValue { get; set; }
            public string ErrorMessage { get; set; }
            public string ID { get; set; }
        }
        
        public class ValidationSummary
        {
            public string ID { get; set; }
        }
        
        public class HyperLink
        {
            public string NavigateUrl { get; set; }
            public string Text { get; set; }
            public string ID { get; set; }
        }
        
        public class Image
        {
            public string ImageUrl { get; set; }
            public string ID { get; set; }
        }

        public class SqlDataSource
        {
            public string ID { get; set; }
            public string ConnectionString { get; set; }
            public string SelectCommand { get; set; }
            public string UpdateCommand { get; set; }
            public string InsertCommand { get; set; }
            public string DeleteCommand { get; set; }
        }
    }
    
    namespace HtmlControls
    {
        public class HtmlForm
        {
            public string ID { get; set; }
        }
        
        public class HtmlInputText
        {
            public string Value { get; set; }
            public string ID { get; set; }
        }
        
        public class HtmlInputFile
        {
            public string ID { get; set; }
        }
        
        public class HtmlGenericControl
        {
            public string InnerText { get; set; }
            public string InnerHtml { get; set; }
            public string ID { get; set; }
        }
    }
}

namespace System.Web
{
    public class HttpRequest
    {
        public HttpCookieCollection Cookies { get; set; }
        public System.Collections.Specialized.NameValueCollection QueryString { get; set; }
        public System.Collections.Specialized.NameValueCollection Form { get; set; }
    }
    
    public class HttpResponse
    {
        public HttpCookieCollection Cookies { get; set; }
        public void Redirect(string url) { }
        public void Write(string s) { }
    }
    
    public class HttpServerUtility
    {
        public string MapPath(string path) { return path; }
        public void Transfer(string path) { }
    }
    
    public class HttpSessionState
    {
        public object this[string name]
        {
            get { return null; }
            set { }
        }
    }
    
    public class HttpApplicationState
    {
        public object this[string name]
        {
            get { return null; }
            set { }
        }
    }
    
    public class HttpCookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    
    public class HttpCookieCollection
    {
        public HttpCookie this[string name]
        {
            get { return null; }
        }
        
        public void Add(HttpCookie cookie) { }
    }
}
