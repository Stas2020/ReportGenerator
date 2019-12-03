﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Этот исходный текст был создан автоматически: Microsoft.VSDesigner, версия: 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace ReportMonthResultGenerator.vfiliaszar8 {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EmployeesSoapBinding", Namespace="http://filias1c.intra.cofemania.ru/1cws")]
    public partial class Employees : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback AddOperationCompleted;
        
        private System.Threading.SendOrPostCallback DelOperationCompleted;
        
        private System.Threading.SendOrPostCallback UnDismissOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetPregnantListOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Employees() {
            this.Url = global::ReportMonthResultGenerator.Properties.Settings.Default.ReportMonthResultGenerator_vfiliaszar8_Employees;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event AddCompletedEventHandler AddCompleted;
        
        /// <remarks/>
        public event DelCompletedEventHandler DelCompleted;
        
        /// <remarks/>
        public event UnDismissCompletedEventHandler UnDismissCompleted;
        
        /// <remarks/>
        public event GetPregnantListCompletedEventHandler GetPregnantListCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://filias1c.intra.cofemania.ru/1cws#Employees:Add", RequestNamespace="http://filias1c.intra.cofemania.ru/1cws", ResponseNamespace="http://filias1c.intra.cofemania.ru/1cws", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string Add(string Name, string SName, string MName, string Code) {
            object[] results = this.Invoke("Add", new object[] {
                        Name,
                        SName,
                        MName,
                        Code});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void AddAsync(string Name, string SName, string MName, string Code) {
            this.AddAsync(Name, SName, MName, Code, null);
        }
        
        /// <remarks/>
        public void AddAsync(string Name, string SName, string MName, string Code, object userState) {
            if ((this.AddOperationCompleted == null)) {
                this.AddOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddOperationCompleted);
            }
            this.InvokeAsync("Add", new object[] {
                        Name,
                        SName,
                        MName,
                        Code}, this.AddOperationCompleted, userState);
        }
        
        private void OnAddOperationCompleted(object arg) {
            if ((this.AddCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddCompleted(this, new AddCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://filias1c.intra.cofemania.ru/1cws#Employees:Del", RequestNamespace="http://filias1c.intra.cofemania.ru/1cws", ResponseNamespace="http://filias1c.intra.cofemania.ru/1cws", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string Del(string EmpCode, [System.Xml.Serialization.XmlElementAttribute(DataType="date")] System.DateTime DateOfDel) {
            object[] results = this.Invoke("Del", new object[] {
                        EmpCode,
                        DateOfDel});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void DelAsync(string EmpCode, System.DateTime DateOfDel) {
            this.DelAsync(EmpCode, DateOfDel, null);
        }
        
        /// <remarks/>
        public void DelAsync(string EmpCode, System.DateTime DateOfDel, object userState) {
            if ((this.DelOperationCompleted == null)) {
                this.DelOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDelOperationCompleted);
            }
            this.InvokeAsync("Del", new object[] {
                        EmpCode,
                        DateOfDel}, this.DelOperationCompleted, userState);
        }
        
        private void OnDelOperationCompleted(object arg) {
            if ((this.DelCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DelCompleted(this, new DelCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://filias1c.intra.cofemania.ru/1cws#Employees:UnDismiss", RequestNamespace="http://filias1c.intra.cofemania.ru/1cws", ResponseNamespace="http://filias1c.intra.cofemania.ru/1cws", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string UnDismiss(string StaffCode) {
            object[] results = this.Invoke("UnDismiss", new object[] {
                        StaffCode});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void UnDismissAsync(string StaffCode) {
            this.UnDismissAsync(StaffCode, null);
        }
        
        /// <remarks/>
        public void UnDismissAsync(string StaffCode, object userState) {
            if ((this.UnDismissOperationCompleted == null)) {
                this.UnDismissOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUnDismissOperationCompleted);
            }
            this.InvokeAsync("UnDismiss", new object[] {
                        StaffCode}, this.UnDismissOperationCompleted, userState);
        }
        
        private void OnUnDismissOperationCompleted(object arg) {
            if ((this.UnDismissCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UnDismissCompleted(this, new UnDismissCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://filias1c.intra.cofemania.ru/1cws#Employees:GetPregnantList", RequestNamespace="http://filias1c.intra.cofemania.ru/1cws", ResponseNamespace="http://filias1c.intra.cofemania.ru/1cws", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string GetPregnantList() {
            object[] results = this.Invoke("GetPregnantList", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetPregnantListAsync() {
            this.GetPregnantListAsync(null);
        }
        
        /// <remarks/>
        public void GetPregnantListAsync(object userState) {
            if ((this.GetPregnantListOperationCompleted == null)) {
                this.GetPregnantListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPregnantListOperationCompleted);
            }
            this.InvokeAsync("GetPregnantList", new object[0], this.GetPregnantListOperationCompleted, userState);
        }
        
        private void OnGetPregnantListOperationCompleted(object arg) {
            if ((this.GetPregnantListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPregnantListCompleted(this, new GetPregnantListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void AddCompletedEventHandler(object sender, AddCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AddCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AddCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void DelCompletedEventHandler(object sender, DelCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DelCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DelCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void UnDismissCompletedEventHandler(object sender, UnDismissCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UnDismissCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UnDismissCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    public delegate void GetPregnantListCompletedEventHandler(object sender, GetPregnantListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1586.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPregnantListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPregnantListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591