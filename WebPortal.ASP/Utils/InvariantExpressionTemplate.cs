using Serilog.Events;
using Serilog.Formatting;
using Serilog.Templates;
using System.Globalization;
using System.IO;
using System.Threading;

namespace WebPortal.ASP.App_Start
{
    //Fixes Exception.ToString() was being localized to arabic if the UI was arabic
    //certian words were getting translated to arabic
    public class InvariantExpressionTemplate : ITextFormatter
    {
        private readonly ExpressionTemplate _template;

        public InvariantExpressionTemplate(string template)
        {
            _template = new ExpressionTemplate(
                template,
                CultureInfo.InvariantCulture);
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            CultureInfo originalCulture =
                Thread.CurrentThread.CurrentCulture;

            CultureInfo originalUICulture =
                Thread.CurrentThread.CurrentUICulture;

            try
            {
                Thread.CurrentThread.CurrentCulture =
                    CultureInfo.InvariantCulture;

                Thread.CurrentThread.CurrentUICulture =
                    CultureInfo.InvariantCulture;

                _template.Format(logEvent, output);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture =
                    originalCulture;

                Thread.CurrentThread.CurrentUICulture =
                    originalUICulture;
            }
        }
    }
}