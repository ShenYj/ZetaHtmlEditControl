﻿namespace ZetaHtmlEditControl.UI.EditControlBases
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using Code.Html;
    using Code.MsHtml;
    using Properties;

    public partial class CoreHtmlEditControl
    {
        private const string CssFontStyle =
            @"font-family: Segoe UI, Tahoma, Verdana, Arial; font-size: {font-size}; ";

        private static string _defaultCssText = @"body { {font-style}; margin: 4px; }
			li { margin-bottom: 5pt; }
			table { {font-style}; }
			th { padding:1px; border-top: 2px inset #777; border-right: 2px inset #fff; border-bottom: 2px inset #aaa; border-left: 2px inset #fff; font-weight: bold; {font-style}; }
			td { padding: 1px; border: 2px inset #fff; {font-style}; }";

        private static string _defaultHtmlTemplate =
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
			<html xmlns=""http://www.w3.org/1999/xhtml"">
				<head>
					<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
					<style type=""text/css"">##CSS##</style>
				</head>
				<body spellcheck=""true"">##BODY##</body>
			</html>";

        private string _cssFontSize;

        private string _cssText = _defaultCssText;
        private HtmlConversionHelper _htmlConversionHelper;
        private string _htmlTemplate = _defaultHtmlTemplate;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string DefaultCssText
        {
            get { return _defaultCssText; }
            set { _defaultCssText = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string DefaultHtmlTemplate
        {
            get { return _defaultHtmlTemplate; }
            set { _defaultHtmlTemplate = value; }
        }

        /// <summary>
        /// Assigns a style sheet to the HTML editor.
        /// Set<see cref="DocumentText"/>t to activate.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CssText
        {
            set
            {
                _cssText = value;
            }
        }

        /// <summary>
        /// Set own HTML Code.
        /// This '##BODY##' Tag will be replaced with the Body.
        /// Optional: '##CSS##'
        /// Set <see cref="DocumentText"/> to activate.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string HtmlTemplate
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(
                        @"value",
                        Resources.SR_HtmlEditControl_HtmlTemplate_AvaluefortheHtmlTemplatemustbeprovided);
                }
                else if (!value.Contains(@"##BODY##"))
                {
                    throw new ArgumentException(
                        Resources.SR_HtmlEditControl_HtmlTemplate_MissingBODYinsidetheHtmlTemplatepropertyvalue,
                        @"value");
                }
                else
                {
                    _htmlTemplate = value;
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CompleteDocumentText
        {
            get { return base.DocumentText; }
            set { base.DocumentText = value; }
        }

        /// <summary>
        /// Gets or sets the HTML content.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string DocumentText
        {
            get
            {
                return prepareDocumentTextGet(MsHtmlLegacyFromBadToGoodTranslator.Translate(base.DocumentText));
            }
            set
            {
                base.DocumentText = prepareDocumentTextSet(MsHtmlLegacyFromGoodToBadTranslator.Translate(value));
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TextOnlyFromDocumentBody
        {
            get
            {
                return base.DocumentText.GetBodyFromHtmlCode().GetOnlyTextFromHtmlCode();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CssFontSize
        {
            get { return string.IsNullOrEmpty(_cssFontSize) ? getCssFontSizeWithUnit() : _cssFontSize; }
            set { _cssFontSize = value; }
        }

        private void constructCoreHtmlEditControlTextAndImage()
        {
            _htmlConversionHelper = new HtmlConversionHelper();
        }

        public string MakeFullHtmlFromBody(
            string body)
        {
            return doBuildCompleteHtml(body, _defaultHtmlTemplate, _defaultCssText);
        }

        public void SetDocumentText(
            string text)
        {
            SetDocumentText(text, null, false);
        }

        public void SetDocumentText(
            string text,
            string externalImagesFolderPath,
            bool useImagesFolderPathPlaceHolder)
        {
            DocumentText =
                _htmlConversionHelper.ConvertSetHtml(
                    text,
                    externalImagesFolderPath,
                    useImagesFolderPathPlaceHolder ? HtmlImageHelper.ImagesFolderPathPlaceHolder : null);
        }

        /// <summary>
        /// Gets the document text and stores images to the given folder.
        /// </summary>
        /// <param name="externalImagesFolderPath">Folder path to store the images.</param>
        /// <returns>Returns the HTML code of the body.</returns>
        public string GetDocumentText(
            string externalImagesFolderPath)
        {
            return GetDocumentText(externalImagesFolderPath, false);
        }

        public string GetDocumentText(
            string externalImagesFolderPath,
            bool useImagesFolderPathPlaceHolder)
        {
            return
                _htmlConversionHelper.ConvertGetHtml(
                    DocumentText,
                    Document == null ? null : Document.Url,
                    externalImagesFolderPath,
                    useImagesFolderPathPlaceHolder ? HtmlImageHelper.ImagesFolderPathPlaceHolder : null);
        }

        private string prepareDocumentTextSet(string html)
        {
            return buildCompleteHtml(html.GetBodyFromHtmlCode());
        }

        private string buildCompleteHtml(string htmlBody)
        {
            return doBuildCompleteHtml(htmlBody, _htmlTemplate, _cssText);
        }

        private string doBuildCompleteHtml(
            string htmlBody,
            string htmlTemplate,
            string cssText)
        {
            string tmpHtml;
            if (string.IsNullOrEmpty(htmlTemplate))
            {
                tmpHtml = htmlBody;
            }
            else
            {
                tmpHtml = htmlTemplate;
                tmpHtml = tmpHtml.Replace(@"##BODY##", htmlBody);
            }

            tmpHtml = tmpHtml.Replace(@"##CSS##", replaceCss(cssText));

            return tmpHtml;
        }

        private string replaceCss(string cssText)
        {
            if (!string.IsNullOrEmpty(cssText) && cssText.Contains(@"{font-style}"))
            {
                var result = cssText.Replace(@"{font-style}", CssFontStyle);
                result = result.Replace(@"{font-size}", CssFontSize);
                return result;
            }
            else
            {
                return cssText;
            }
        }

        private string getCssFontSizeWithUnit()
        {
            //Console.WriteLine(getFontScaleFactor());

            // http://stackoverflow.com/questions/139655/convert-pixels-to-points
            var font = Font;

            switch (font.Unit)
            {
                case GraphicsUnit.World:
                case GraphicsUnit.Display:
                case GraphicsUnit.Inch:
                case GraphicsUnit.Document:
                case GraphicsUnit.Millimeter:
                    return string.Format(@"{0}pt", font.SizeInPoints);
                case GraphicsUnit.Pixel:
                    return string.Format(@"{0}px", font.Size);
                case GraphicsUnit.Point:
                    return string.Format(@"{0}pt", font.Size);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float getFontScaleFactor()
        {
            var myFont = Font;
            var sysFont = SystemFonts.DialogFont;

            var sizeInPixel1 = getSizeInPixel(myFont);
            var sizeInPixel2 = getSizeInPixel(sysFont);

            var factor = sizeInPixel1/sizeInPixel2;
            return factor;
        }

        private float getSizeInPixel(Font f)
        {
            if (f.Unit == GraphicsUnit.Pixel)
            {
                return f.Size;
            }
            else
            {
                var dpi = getDpi();
                return f.SizeInPoints/72*dpi;
            }
        }

        private float _dpi;
        private float getDpi()
        {
            if (_dpi <= 0)
            {
                using (var gfx = CreateGraphics())
                {
                    _dpi = gfx.DpiY;
                }
            }

            return _dpi;
        }

        private static string prepareDocumentTextGet(string html)
        {
            var s = html.GetBodyFromHtmlCode();
            s = Regex.Replace(s, @"<![^>]*>", string.Empty, RegexOptions.Singleline);
            return s;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_htmlConversionHelper != null)
            {
                ((IDisposable)_htmlConversionHelper).Dispose();
                _htmlConversionHelper = null;
            }
        }
    }
}