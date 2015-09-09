namespace HNAS.Framework4.Logging
{
    /// <summary>
    /// 日志类型
    /// </summary>
    /// 创 建 人：胡鹏
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    public static class WriteType
    {
        /// <summary>
        /// 写数据库
        /// </summary>
        /// 创 建 人：胡鹏
        /// 创建日期：2012-11-28
        /// 修 改 人：
        /// 修改日期：
        public static string CreateDBLog()
        {
            string codeFormat = CodeCommon.GetFileMaster(CodeCommon.WebUIPath, "web.Config");
            codeFormat = codeFormat.Replace(" [$loggingConfiguration$]", @"<loggingConfiguration name='Logging Application Block' tracingEnabled='true' defaultCategory='General' logWarningsWhenNoCategoriesMatch='true'>
		    <listeners>
			    <add name='Event Log Listener' type='Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.FormattedEventLogTraceListener, Microsoft.Practices.EnterpriseLibrary.Logging' listenerDataType='Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.FormattedEventLogTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging' source='Enterprise Library Logging' formatter='Text Formatter' log='Application' machineName='' traceOutputOptions='None' filter='All'/>
			    <add name='Database Trace Listener' type='Microsoft.Practices.EnterpriseLibrary.Logging.Database.FormattedDatabaseTraceListener, Microsoft.Practices.EnterpriseLibrary.Logging.Database, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null' listenerDataType='Microsoft.Practices.EnterpriseLibrary.Logging.Database.Configuration.FormattedDatabaseTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging.Database, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null' databaseInstanceName='dblog' writeLogStoredProcName='WriteLog' addCategoryStoredProcName='AddCategory' formatter='Text Formatter'/>
		    </listeners>
		    <formatters>
			    <add type='Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.TextFormatter, Microsoft.Practices.EnterpriseLibrary.Logging' template='Timestamp: {timestamp}&#xA;Message: {message}&#xA;Category: {category}&#xA;Priority: {priority}&#xA;EventId: {eventid}&#xA;Severity: {severity}&#xA;Title:{title}&#xA;Machine: {machine}&#xA;Application Domain: {appDomain}&#xA;Process Id: {processId}&#xA;Process Name: {processName}&#xA;Win32 Thread Id: {win32ThreadId}&#xA;Thread Name: {threadName}&#xA;Extended Properties: {dictionary({key} - {value}&#xA;)}' name='Text Formatter'/>
		    </formatters>
		    <categorySources>
			    <add switchValue='All' name='General'>
				    <listeners>
					    <add name='Database Trace Listener'/>
				    </listeners>
			    </add>
		    </categorySources>
		    <specialSources>
			    <allEvents switchValue='All' name='All Events'/>
			    <notProcessed switchValue='All' name='Unprocessed Category'/>
			    <errors switchValue='All' name='Logging Errors &amp; Warnings'>
				    <listeners>
					    <add name='Event Log Listener'/>
				    </listeners>
			    </errors>
		    </specialSources>
	        </loggingConfiguration>");
            return codeFormat;
        }
        /// <summary>
        /// 写文本
        /// </summary>
        /// 创 建 人：胡鹏
        /// 创建日期：2012-11-28
        /// 修 改 人：
        /// 修改日期：
        public static string CreateTxtLog()
        {
            string codeFormat = CodeCommon.GetFileMaster(CodeCommon.WebUIPath, "web.Config");
            codeFormat = codeFormat.Replace(" [$loggingConfiguration$]", @"<loggingConfiguration name='Logging Application Block' tracingEnabled='true' defaultCategory='General' logWarningsWhenNoCategoriesMatch='true'>
            <listeners>
              <add name='Event Log Listener' type='Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.FormattedEventLogTraceListener, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null' listenerDataType='Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.FormattedEventLogTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null'
              source='Enterprise Library Logging' formatter='Text Formatter'
                              log='' machineName='.' traceOutputOptions='None' />
              <add name='Rolling Flat File Trace Listener' type='Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.FlatFileTraceListener, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null'
              listenerDataType='Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.FlatFileTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null'
              fileName='Logs\trace.log' header='新的一条日志：' formatter='Text Formatter' rollInterval='Day' maxArchivedFiles='100' timeStampPattern='yyyy-MM-dd' traceOutputOptions='DateTime'/>
            </listeners>
            <formatters>
              <add type='Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.TextFormatter, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.414.0, Culture=neutral, PublicKeyToken=null'
                              template='Timestamp: {timestamp}{newline}&#xA;Message: {message}{newline}&#xA;Title:{title}{newline}&#xA;Machine: {localMachine}{newline}&#xA;App Domain: {localAppDomain}{newline}&#xA;Process Name: {localProcessName}{newline}&#xA;Thread Name: {threadName}{newline}&#xA;Extended Properties: {dictionary({key} - {value}{newline})}'
              name='Text Formatter' />
            </formatters>
            <categorySources>
              <add switchValue='All' name='General'>
                <listeners>
                  <add name='Flat File Trace Listener' />
                </listeners>
              </add>
            </categorySources>
            <specialSources>
              <allEvents switchValue='All' name='All Events' />
              <notProcessed switchValue='All' name='Unprocessed Category' />
              <errors switchValue='All' name='Logging Errors &amp; Warnings'>
                <listeners>
                  <add name='Event Log Listener' />
                </listeners>
              </errors>
            </specialSources>
          </loggingConfiguration>");
            return codeFormat;
        }
    }
}
