using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Licitacion.Servicios
{
    public class ServicioPDF
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";
        public void generarFichaObra(string rutaCompleta, int? idObra)
        {
            ServicioLicitacion servicioLicitacion = new ServicioLicitacion();
            var unLicitacion = servicioLicitacion.buscarUna(idObra);
            //directorio = HttpContext.Current.Server.MapPath("~/TempFiles/");
            // Creamos el documento con el tamaño de página tradicional
            PdfWriter writer = new PdfWriter(rutaCompleta);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            #region Font 
            // Creamos el tipo de Font que vamos utilizar
            Style _standardFont = new Style();
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.HELVETICA);
            _standardFont.SetFont(font).SetFontSize(10);

            Style _standardFontChica = new Style();
            PdfFont font2 = PdfFontFactory.CreateFont(FontConstants.HELVETICA);
            _standardFontChica.SetFont(font2).SetFontSize(9);

            Style _headerFont = new Style();
            PdfFont font3 = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);
            _headerFont.SetFont(font3).SetFontSize(11);

            Style _titleFont = new Style();
            PdfFont font4 = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);
            _titleFont.SetFont(font4).SetFontSize(10);

            Style _emptyFont = new Style();
            PdfFont font5 = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);
            _emptyFont.SetFont(font5).SetFontSize(2);

            #endregion

            #region Imagen
            
            string imageUrl = System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString();
            Image img = new Image(ImageDataFactory
                       .Create(imageUrl))
                       .SetTextAlignment(TextAlignment.CENTER);
            
            Cell celImage = new Cell();

            celImage.SetTextAlignment(TextAlignment.CENTER);
            celImage.Add(img.SetWidth(50));           
            celImage.SetBorder(Border.NO_BORDER);

            //img.ScaleAbsoluteWidth(200);
            //img.ScaleAbsoluteHeight(50);
            #endregion

            #region Encabezado
            Table tblHeader = new Table(1);
            Cell clNombre2 = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph("FICHA DE OBRA DE: " + unLicitacion.nombreObra))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                //.SetBorderLeft(Border.ROUND_DOTS)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .AddStyle(_headerFont)
                .SetBorderTop(Border.NO_BORDER);

            tblHeader.AddCell(clNombre2);


            /////////////////////////////////////////////////////////
            #endregion

            Table tblCuerpo = new Table(3);

            Cell clNro1 = new Cell(1, 3)
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph("Expediente: " + unLicitacion.nroExpedienteString))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetBorderLeft(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .AddStyle(_standardFont)
                .SetHeight(30f)
                .SetBorderTop(Border.NO_BORDER);

            Cell clNro10 = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph(" "))
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .AddStyle(_standardFont)
                .SetHeight(30f)
                .SetBorderTop(Border.NO_BORDER);

            Cell clNro11 = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph(" "))
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER)
                .AddStyle(_standardFont)
                .SetHeight(30f)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorderTop(Border.NO_BORDER);

            tblCuerpo.AddCell(clNro1);

            Cell clLabelOrden = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph("Descripcion: " + unLicitacion.descripcion))
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .AddStyle(_standardFont)
                .SetHeight(30f);

            Cell clLabelNombre = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph("Estado: " + unLicitacion.nombreEtapa))
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .AddStyle(_standardFont)
                .SetHeight(30f);

            Cell clLabelFirma = new Cell()
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(new Paragraph("Organismo: " + unLicitacion.nombreOrganismoString))
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderBottom(Border.NO_BORDER)
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .AddStyle(_standardFont)
                .SetHeight(30f);


            //PdfPCell clLabelAclaracion = new PdfPCell(new Phrase("Presupuesto Oficial: " + unLicitacion.montoObraString, _standardFont));
            //clLabelAclaracion.HorizontalAlignment = Element.ALIGN_LEFT;
            //clLabelAclaracion.VerticalAlignment = Element.ALIGN_MIDDLE;
            ////clLabelAclaracion.BackgroundColor = new iTextSharp.text.BaseColor(224, 224, 224);
            ////cltitulo.BorderWidthBottom = 0.75f;
            //clLabelAclaracion.BorderWidthBottom = 0;
            //clLabelAclaracion.BorderWidthLeft = 0;
            //clLabelAclaracion.BorderWidthTop = 0;
            //clLabelAclaracion.BorderWidthRight = 0;
            //clLabelAclaracion.FixedHeight = 30f;

            //PdfPCell clMoneda = new PdfPCell(new Phrase("Moneda: " + unLicitacion.nombreMoneda, _standardFont));
            //clMoneda.HorizontalAlignment = Element.ALIGN_LEFT;
            //clMoneda.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clMoneda.BorderWidthBottom = 0;
            //clMoneda.BorderWidthLeft = 0;
            //clMoneda.BorderWidthTop = 0;
            //clMoneda.BorderWidthRight = 0;
            //clMoneda.FixedHeight = 30f;

            //PdfPCell clContratacion = new PdfPCell(new Phrase("Tipo Contratacion: " + unLicitacion.nombreTipoContratacion, _standardFont));
            //clContratacion.HorizontalAlignment = Element.ALIGN_LEFT;
            //clContratacion.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clContratacion.BorderWidthBottom = 0;
            //clContratacion.BorderWidthLeft = 0;
            //clContratacion.BorderWidthTop = 0;
            //clContratacion.BorderWidthRight = 0;
            //clContratacion.FixedHeight = 30f;

            //PdfPCell clEspecialidad = new PdfPCell(new Phrase("Especialidad: " + "", _standardFont));
            //clEspecialidad.HorizontalAlignment = Element.ALIGN_LEFT;
            //clEspecialidad.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clEspecialidad.BorderWidthBottom = 0;
            //clEspecialidad.BorderWidthLeft = 0;
            //clEspecialidad.BorderWidthTop = 0;
            //clEspecialidad.BorderWidthRight = 0;
            //clEspecialidad.Colspan = 1;
            //clEspecialidad.FixedHeight = 30f;

            //PdfPCell clSubEspecialidad = new PdfPCell(new Phrase("Subespecialidad: " + "", _standardFont));
            //clSubEspecialidad.HorizontalAlignment = Element.ALIGN_LEFT;
            //clSubEspecialidad.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clSubEspecialidad.BorderWidthBottom = 0;
            //clSubEspecialidad.BorderWidthLeft = 0;
            //clSubEspecialidad.BorderWidthTop = 0;
            //clSubEspecialidad.BorderWidthRight = 0;
            //clSubEspecialidad.FixedHeight = 30f;
            //clSubEspecialidad.Colspan = 2;

            //PdfPCell clEjecucion = new PdfPCell(new Phrase("Plazo Ejecución: " + unLicitacion.plazo, _standardFont));
            //clEjecucion.HorizontalAlignment = Element.ALIGN_LEFT;
            //clEjecucion.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clEjecucion.BorderWidthBottom = 0;
            //clEjecucion.BorderWidthLeft = 0;
            //clEjecucion.BorderWidthTop = 0;
            //clEjecucion.BorderWidthRight = 0;
            //clEjecucion.FixedHeight = 30f;

            //PdfPCell clDomicilio = new PdfPCell(new Phrase("Domicilio: " + unLicitacion.domicilio, _standardFont));
            //clDomicilio.HorizontalAlignment = Element.ALIGN_LEFT;
            //clDomicilio.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clDomicilio.BorderWidthBottom = 0;
            //clDomicilio.BorderWidthLeft = 0;
            //clDomicilio.BorderWidthTop = 0;
            //clDomicilio.BorderWidthRight = 0;
            //clDomicilio.Colspan = 2;
            //clDomicilio.FixedHeight = 30f;

            //PdfPCell clDepartamento = new PdfPCell(new Phrase("Departamento: " + unLicitacion.departamento, _standardFont)); 
            //clDepartamento.HorizontalAlignment = Element.ALIGN_LEFT;
            //clDepartamento.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clDepartamento.BorderWidthBottom = 0;
            //clDepartamento.BorderWidthLeft = 0;
            //clDepartamento.BorderWidthTop = 0;
            //clDepartamento.BorderWidthRight = 0;
            //clDepartamento.FixedHeight = 30f;

            //PdfPCell clUrlPliego = new PdfPCell(new Phrase("Pliego: " + unLicitacion.urlPliego, _standardFont));
            //clUrlPliego.HorizontalAlignment = Element.ALIGN_LEFT;
            //clUrlPliego.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clUrlPliego.BorderWidthBottom = 0;
            //clUrlPliego.BorderWidthLeft = 0;
            //clUrlPliego.BorderWidthTop = 0;
            //clUrlPliego.BorderWidthRight = 0;
            //clUrlPliego.FixedHeight = 30f;
            //clUrlPliego.Colspan = 2;

            //PdfPCell clValorPliego = new PdfPCell(new Phrase("Valor Pliego: " + unLicitacion.valorPliego, _standardFont)); 
            //clValorPliego.HorizontalAlignment = Element.ALIGN_LEFT;
            //clValorPliego.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clValorPliego.BorderWidthBottom = 0;
            //clValorPliego.BorderWidthLeft = 0;
            //clValorPliego.BorderWidthTop = 0;
            //clValorPliego.BorderWidthRight = 0;
            //clValorPliego.FixedHeight = 30f;

            //PdfPCell clFechaVisita = new PdfPCell(new Phrase("Fecha Visita: " + unLicitacion.fechaVisitaString, _standardFont)); 
            //clFechaVisita.HorizontalAlignment = Element.ALIGN_LEFT;
            //clFechaVisita.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clFechaVisita.BorderWidthBottom = 0;
            //clFechaVisita.BorderWidthLeft = 0;
            //clFechaVisita.BorderWidthTop = 0;
            //clFechaVisita.BorderWidthRight = 0;
            //clFechaVisita.FixedHeight = 30f;


            //PdfPCell clLugarVisita = new PdfPCell(new Phrase("Lugar Visita: " + unLicitacion.lugarVisita, _standardFont));
            //clLugarVisita.HorizontalAlignment = Element.ALIGN_LEFT;
            //clLugarVisita.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clLugarVisita.BorderWidthBottom = 0;
            //clLugarVisita.BorderWidthLeft = 0;
            //clLugarVisita.BorderWidthTop = 0;
            //clLugarVisita.BorderWidthRight = 0;
            //clLugarVisita.Colspan = 2;
            //clLugarVisita.FixedHeight = 30f;


            //PdfPCell clFechaCierre = new PdfPCell(new Phrase("Fecha Cierre: " + unLicitacion.fechaCierreString, _standardFont));
            //clFechaCierre.HorizontalAlignment = Element.ALIGN_LEFT;
            //clFechaCierre.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clFechaCierre.BorderWidthBottom = 0;
            //clFechaCierre.BorderWidthLeft = 0;
            //clFechaCierre.BorderWidthTop = 0;
            //clFechaCierre.BorderWidthRight = 0;
            //clFechaCierre.FixedHeight = 30f;

            //var fecha = unLicitacion.fechaVisita;

            //PdfPCell clFechaApertura = new PdfPCell(new Phrase("Fecha Apertura: " + unLicitacion.fechaAperturaString, _standardFont));
            //clFechaApertura.HorizontalAlignment = Element.ALIGN_LEFT;
            //clFechaApertura.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clFechaApertura.BorderWidthBottom = 0;
            //clFechaApertura.BorderWidthLeft = 0;
            //clFechaApertura.BorderWidthTop = 0;
            //clFechaApertura.BorderWidthRight = 0;
            //clFechaApertura.FixedHeight = 30f;

            //PdfPCell clHoraApertura = new PdfPCell(new Phrase("Hora Apertura: " + unLicitacion.horaApertura, _standardFont));
            //clHoraApertura.HorizontalAlignment = Element.ALIGN_LEFT;
            //clHoraApertura.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clHoraApertura.BorderWidthBottom = 0;
            //clHoraApertura.BorderWidthLeft = 0;
            //clHoraApertura.BorderWidthTop = 0;
            //clHoraApertura.BorderWidthRight = 0;
            //clHoraApertura.FixedHeight = 30f;

            //PdfPCell clDomPres = new PdfPCell(new Phrase("Domicilio Presentación: " + unLicitacion.domicilioPresentacion, _standardFont));
            //clDomPres.HorizontalAlignment = Element.ALIGN_LEFT;
            //clDomPres.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clDomPres.BorderWidthBottom = 0;
            //clDomPres.BorderWidthLeft = 0;
            //clDomPres.BorderWidthTop = 0;
            //clDomPres.BorderWidthRight = 0;
            //clDomPres.Colspan = 3;
            //clDomPres.FixedHeight = 30f;

            //PdfPCell clDomAper = new PdfPCell(new Phrase("Domicilio Apertura: " + unLicitacion.domicilioApertura, _standardFont));
            //clDomAper.HorizontalAlignment = Element.ALIGN_LEFT;
            //clDomAper.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clDomAper.BorderWidthBottom = 0;
            //clDomAper.BorderWidthLeft = 0;
            //clDomAper.BorderWidthTop = 0;
            //clDomAper.BorderWidthRight = 0;
            //clDomAper.Colspan = 3;
            //clDomAper.FixedHeight = 30f;

            //PdfPCell clMailConsulta = new PdfPCell(new Phrase("Mail Consultas: " + unLicitacion.mailConsulta, _standardFont));
            //clMailConsulta.HorizontalAlignment = Element.ALIGN_LEFT;
            //clMailConsulta.VerticalAlignment = Element.ALIGN_MIDDLE;
            //clMailConsulta.BorderWidthBottom = 0;
            //clMailConsulta.BorderWidthLeft = 0;
            //clMailConsulta.BorderWidthTop = 0;
            //clMailConsulta.BorderWidthRight = 0;
            //clMailConsulta.FixedHeight = 30f;
            //clMailConsulta.Colspan = 3;
            ///////////////////
            tblCuerpo.AddCell(clLabelOrden);

            tblCuerpo.AddCell(clLabelNombre);
            tblCuerpo.AddCell(clLabelFirma);

            //tblCuerpo.AddCell(clLabelAclaracion);
            //tblCuerpo.AddCell(clMoneda);
            //tblCuerpo.AddCell(clNro10);

            //tblCuerpo.AddCell(clContratacion);
            //tblCuerpo.AddCell(clEjecucion);
            //tblCuerpo.AddCell(clNro10);

            //tblCuerpo.AddCell(clEspecialidad);
            //tblCuerpo.AddCell(clSubEspecialidad);

            //tblCuerpo.AddCell(clDomicilio);
            //tblCuerpo.AddCell(clDepartamento);

            //tblCuerpo.AddCell(clUrlPliego);
            //tblCuerpo.AddCell(clValorPliego);
            

            //tblCuerpo.AddCell(clFechaVisita);
            //tblCuerpo.AddCell(clLugarVisita);

            //tblCuerpo.AddCell(clFechaCierre);
            //tblCuerpo.AddCell(clFechaApertura);
            //tblCuerpo.AddCell(clHoraApertura);

            //tblCuerpo.AddCell(clDomPres);
            //tblCuerpo.AddCell(clDomAper);
            //tblCuerpo.AddCell(clMailConsulta);

            Table tblHeader2 = new Table(1);

            tblHeader2.AddCell(tblCuerpo);
            // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
            document.Add(img);
            document.Add(tblHeader);
            document.Add(tblHeader2);
            document.Close();
            writer.Close();

        }
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
        public void generarActaApertura(string rutaCompleta, ActaViewModels unActa)
        {
            try
            {

                PdfWriter writer = new PdfWriter(rutaCompleta);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                document.SetMargins(60, 36, 36, 36);
                pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderFooterEventHandler());

                #region Font 
                //Creamos el tipo de Font que vamos utilizar
                //Style _emptyFont = new Style();
                //PdfFont font5 = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                //_emptyFont.SetFont(font5).SetFontSize(2);

                //Style _standardFont = new Style();
                //PdfFont font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
                //_standardFont.SetFont(font).SetFontSize(10);

                //Style _standardFontChica = new Style();
                //PdfFont font2 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
                //_standardFontChica.SetFont(font2).SetFontSize(9);

                //Style _headerFont = new Style();
                //PdfFont font3 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                //_headerFont.SetFont(font3).SetFontSize(10);

                //Style _titleFont = new Style();
                //PdfFont font4 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                //_titleFont.SetFont(font4).SetFontSize(10);

                #endregion

                #region Imagen

                string imageUrl = System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString();
                Image img = new Image(ImageDataFactory
                           .Create(imageUrl))
                           .SetTextAlignment(TextAlignment.CENTER);

                Cell celImage = new Cell();

                celImage.SetTextAlignment(TextAlignment.CENTER);
                celImage.Add(img.SetWidth(200));
                celImage.SetBorder(Border.NO_BORDER);

                #endregion

                #region Encabezado
                var textoEncabezado = HtmlToPlainText(unActa.textoEncabezado);
                textoEncabezado = textoEncabezado.Replace("\r\n", "|");
                var arrayEncabezado = textoEncabezado.Split('|');

                Table tblHeader = new Table(1);
                Cell espacio = new Cell()
                                .SetTextAlignment(TextAlignment.LEFT)
                                .Add(new Paragraph("  "))
                                .SetBorderRight(Border.NO_BORDER)
                                .SetBorderTop(Border.NO_BORDER)
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderBottom(Border.NO_BORDER)
                                .SetHeight(10f);
                tblHeader.AddCell(espacio);
                //tblHeader.AddCell(espacio);
                tblHeader.SetMarginTop(50);
                foreach (var item in arrayEncabezado)
                {
                    if(!string.IsNullOrEmpty(item))
                    {
                        Cell clNombre2 = new Cell()
                            .SetTextAlignment(TextAlignment.LEFT)
                            .Add(new Paragraph(item))
                            //.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetBorderRight(Border.NO_BORDER)
                            .SetBorderTop(Border.NO_BORDER)
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderBottom(Border.NO_BORDER)
                            //.SetBorderTopLeftRadius(new BorderRadius(6))
                            //.SetBorderTopRightRadius(new BorderRadius(6))
                            //.SetBorderBottomLeftRadius(new BorderRadius(6))
                            //.SetBorderBottomRightRadius(new BorderRadius(6))
                            //.SetBorder(new RoundDotsBorder(ColorConstants.BLACK,1))
                            //.SetBorderBottom((new SolidBorder(ColorConstants.BLACK, 4)))                    
                            //.SetBorderRadius(new BorderRadius(6))
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            //.SetFontFamily(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN)
                            //.SetFontFamily(StandardFonts.TIMES_BOLD)
                            .SetFontSize(10);
                            
                            //.AddStyle(_headerFont);
                            //.SetHeight(20f);

                        tblHeader.AddCell(clNombre2);
                    }
                }
                #endregion

                #region Cuerpo
                var textoOferta = unActa.textoOferta.Replace("<ul>", "{");
                textoOferta = textoOferta.Replace("</ul>", "{");
                textoOferta = textoOferta.Replace("<li>", "{");
                textoOferta = textoOferta.Replace("</li>", "{");
                textoOferta = textoOferta.Replace("<p>", "{");
                textoOferta = textoOferta.Replace("</p>", "{");
                textoOferta = textoOferta.Replace("\r\n", "{");
                //textoOferta = HtmlToPlainText(textoOferta);
                var arrayOferta = textoOferta.Split('{');

                foreach (var item in arrayOferta)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var texto = HtmlToPlainText(item);
                        Cell clNombre2 = new Cell()
                            .SetTextAlignment(TextAlignment.LEFT)
                            .Add(new Paragraph(texto))
                            //.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetBorderRight(Border.NO_BORDER)
                            .SetBorderTop(Border.NO_BORDER)
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderBottom(Border.NO_BORDER)
                            //.SetBorderTopLeftRadius(new BorderRadius(6))
                            //.SetBorderTopRightRadius(new BorderRadius(6))
                            //.SetBorderBottomLeftRadius(new BorderRadius(6))
                            //.SetBorderBottomRightRadius(new BorderRadius(6))
                            //.SetBorder(new RoundDotsBorder(ColorConstants.BLACK,1))
                            //.SetBorderBottom((new SolidBorder(ColorConstants.BLACK, 4)))                    
                            //.SetBorderRadius(new BorderRadius(6))
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            //.SetFontFamily(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN)
                            .SetFontSize(10);
                            //.AddStyle(_standardFont);
                        
                        if (texto.StartsWith("Oferta"))
                        {
                            Cell clVacia = new Cell()
                                .SetTextAlignment(TextAlignment.LEFT)
                                .Add(new Paragraph("  "))
                                .SetBorderRight(Border.NO_BORDER)
                                .SetBorderTop(Border.NO_BORDER)
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderBottom(Border.NO_BORDER)
                                .SetHeight(10f);
                            tblHeader.AddCell(clVacia);

                            clNombre2.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                                //.AddStyle(_headerFont);
                        }
                        tblHeader.AddCell(clNombre2);
                    }
                }
                #endregion

                #region CuadroResumen
                Table tblCuadro1 = new Table(1);
                Table tblCuadro2 = new Table(1);
                //tblCuadros.AddCell(espacio);
                if (unActa.cuadroResumen != "" && unActa.cuadroResumen != null)
                {
                    PdfPage page = pdf.AddNewPage();
                    //document.Add(new AreaBreak());
                    PageSize pageSize = PageSize.A4.Rotate(); // Rotar la página a horizontal (A4 en formato horizontal)
                    pdf.SetDefaultPageSize(pageSize);
                    
                    //document.Add(img);
                    Cell tituloRes = new Cell()
                                .SetTextAlignment(TextAlignment.LEFT)
                                .Add(new Paragraph("Cuadro Resumen"))
                                //.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                                .SetBorderRight(Border.NO_BORDER)
                                .SetBorderTop(Border.NO_BORDER)
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderBottom(Border.NO_BORDER)
                                //.SetBorderTopLeftRadius(new BorderRadius(6))
                                //.SetBorderTopRightRadius(new BorderRadius(6))
                                //.SetBorderBottomLeftRadius(new BorderRadius(6))
                                //.SetBorderBottomRightRadius(new BorderRadius(6))
                                //.SetBorder(new RoundDotsBorder(ColorConstants.BLACK,1))
                                //.SetBorderBottom((new SolidBorder(ColorConstants.BLACK, 4)))                    
                                //.SetBorderRadius(new BorderRadius(6))
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                //.SetFontFamily(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN)
                                .SetFontSize(12);
                    tituloRes.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    tblCuadro1.AddCell(tituloRes);

                    //el cuadro
                    string[] filas = unActa.cuadroResumen.Split(new string[] { "<tr>" }, StringSplitOptions.RemoveEmptyEntries);

                    int maxColumnas = 0;

                    foreach (string fila in filas)
                    {
                        string[] celdas = fila.Split(new string[] { "<td>" }, StringSplitOptions.RemoveEmptyEntries);

                        maxColumnas = Math.Max(maxColumnas, celdas.Length);
                    }
                    float[] numerosFlotantes = new float[maxColumnas];
                    for (int i = 0; i < maxColumnas; i++)
                    {
                        numerosFlotantes[i] = 40.0f;
                    }
                    Table cuadroResumen = new Table(UnitValue.CreatePercentArray(numerosFlotantes));
                    cuadroResumen.SetWidth(UnitValue.CreatePercentValue(100));

                    for (int i = 1; i < filas.Count(); i++)
                    {
                        var cadaFila = filas[i].Replace("<td>", "{");
                        var arrayFila = cadaFila.Split('{');
                        for (int j = 1; j < arrayFila.Count(); j++)
                        {
                            var itemInsertado = arrayFila[j].Replace("</tr>", "");
                            itemInsertado = itemInsertado.Replace("</td>", "");
                            itemInsertado = itemInsertado.Replace("</br>", "");
                            itemInsertado = itemInsertado.Replace("<br>", "");
                            itemInsertado = itemInsertado.Replace("</tbody>", "");
                            itemInsertado = itemInsertado.Replace("</table>", "");
                            itemInsertado = itemInsertado.Replace("<p>", "");
                            itemInsertado = itemInsertado.Replace("</p>", "");
                            cuadroResumen.AddCell(itemInsertado);
                            var verificarTamanio = maxColumnas;
                            if (arrayFila[j].Equals(arrayFila.Last()))
                            {
                                if (arrayFila.Count() < verificarTamanio)
                                {
                                    int celdasFaltantes = verificarTamanio - (arrayFila.Length - 1);

                                    for (int k = 0; k < celdasFaltantes; k++)
                                    {
                                        cuadroResumen.AddCell("");
                                    }
                                }
                            }

                        }

                    }
                    tblCuadro1.AddCell(cuadroResumen);
                    tblCuadro1.AddCell(espacio);
                }
                
                //
                #endregion
                #region CuadroOferta
                if(unActa.cuadroOferta != "" && unActa.cuadroOferta != null)
                {
                    //document.Add(new AreaBreak());
                    Cell tituloCuadroOferta = new Cell()
                            .SetTextAlignment(TextAlignment.LEFT)
                            .Add(new Paragraph("Cuadro Oferta"))
                            //.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetBorderRight(Border.NO_BORDER)
                            .SetBorderTop(Border.NO_BORDER)
                            .SetBorderLeft(Border.NO_BORDER)
                            .SetBorderBottom(Border.NO_BORDER)
                            //.SetBorderTopLeftRadius(new BorderRadius(6))
                            //.SetBorderTopRightRadius(new BorderRadius(6))
                            //.SetBorderBottomLeftRadius(new BorderRadius(6))
                            //.SetBorderBottomRightRadius(new BorderRadius(6))
                            //.SetBorder(new RoundDotsBorder(ColorConstants.BLACK,1))
                            //.SetBorderBottom((new SolidBorder(ColorConstants.BLACK, 4)))                    
                            //.SetBorderRadius(new BorderRadius(6))
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            //.SetFontFamily(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN)
                            .SetFontSize(12);
                    tituloCuadroOferta.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    tblCuadro2.AddCell(tituloCuadroOferta);

                    //Table tblCuadroOferta = new Table(UnitValue.CreatePercentArray(new float[] { 15, 15, 20, 20 }));
                    //tblCuadroOferta.SetWidth(UnitValue.CreatePercentValue(100));
                    //var cuadroOferta = unActa.cuadroOferta.Replace("<table class=\"table table-bordered\">", "{");
                    //cuadroOferta = cuadroOferta.Replace("</table>", "{");
                    //cuadroOferta = cuadroOferta.Replace("<tbody>", "{");
                    //cuadroOferta = cuadroOferta.Replace("</tbody>", "{");
                    //cuadroOferta = cuadroOferta.Replace("<tr>", "{");
                    //cuadroOferta = cuadroOferta.Replace("</tr>", "{");
                    //cuadroOferta = cuadroOferta.Replace("<td>", "{");
                    //cuadroOferta = cuadroOferta.Replace("</td>", "{");
                    //cuadroOferta = cuadroOferta.Replace("</br>", "");
                    //cuadroOferta = cuadroOferta.Replace("<p>", "");
                    //cuadroOferta = cuadroOferta.Replace("</p>", "{");
                    //cuadroOferta = cuadroOferta.Replace("\r\n", "{");
                    ////textoOferta = HtmlToPlainText(textoOferta);
                    //var arrayCuadroOferta = cuadroOferta.Split('{');
                    string[] filasOferta = unActa.cuadroOferta.Split(new string[] { "<tr>" }, StringSplitOptions.RemoveEmptyEntries);

                    int maxColumnasOferta = 0;
                    foreach (string fila in filasOferta)
                    {
                        string[] celdas = fila.Split(new string[] { "<td>" }, StringSplitOptions.RemoveEmptyEntries);

                        maxColumnasOferta = Math.Max(maxColumnasOferta, celdas.Length);
                    }
                    float[] numerosFlotantesOferta = new float[maxColumnasOferta];
                    for (int i = 0; i < maxColumnasOferta; i++)
                    {
                        numerosFlotantesOferta[i] = 20.0f;
                    }
                    Table cuadroOferta = new Table(UnitValue.CreatePercentArray(numerosFlotantesOferta));
                    cuadroOferta.SetWidth(UnitValue.CreatePercentValue(100));
                    for (int i = 1; i < filasOferta.Count(); i++)
                    {
                        var cadaFila = filasOferta[i].Replace("<td>", "{");
                        var arrayFila = cadaFila.Split('{');
                        for(int item = 1; item < arrayFila.Count (); item++)
                        {
                            var itemInsertar = arrayFila[item].Replace("</td>", "");
                            itemInsertar = itemInsertar.Replace("</tr>", "");
                            itemInsertar = itemInsertar.Replace("</br>", "");
                            itemInsertar = itemInsertar.Replace("<br>", "");
                            itemInsertar = itemInsertar.Replace("</tbody>", "");
                            itemInsertar = itemInsertar.Replace("</table>", "");
                            itemInsertar = itemInsertar.Replace("<p>", "");
                            itemInsertar = itemInsertar.Replace("</p>", "");
                            cuadroOferta.AddCell(itemInsertar);
                        }
                    }

                    tblCuadro2.AddCell(cuadroOferta);
                    tblCuadro2.AddCell(espacio);
                }
                
                 
                //}
                //tblHeader.AddCell(tblCuadroOferta);
                #endregion
                #region Cierre
                Cell clVacia2 = new Cell()
                    .SetTextAlignment(TextAlignment.LEFT)
                    .Add(new Paragraph(""))
                    .SetBorderRight(Border.NO_BORDER)
                    .SetBorderTop(Border.NO_BORDER)
                    .SetBorderLeft(Border.NO_BORDER)
                    .SetBorderBottom(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetFontSize(10);
                    //.AddStyle(_standardFont);

                //tblHeader.AddCell(clVacia2);

                var textoCierre = HtmlToPlainText(unActa.textoCierre);
                Cell clCierre = new Cell()
                    .SetTextAlignment(TextAlignment.LEFT)
                    .Add(new Paragraph(textoCierre))
                    .SetBorderRight(Border.NO_BORDER)
                    .SetBorderTop(Border.NO_BORDER)
                    .SetBorderLeft(Border.NO_BORDER)
                    .SetBorderBottom(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    .SetFontSize(10);
                //.AddStyle(_standardFont);
                tblCuadro2.AddCell(clCierre);
                #endregion

                //document.Add(img);
                document.Add(tblHeader);
                document.Add(tblCuadro1);
                document.Add(new AreaBreak());
                document.Add(tblCuadro2);
                document.Close();
                writer.Close();
                writer.Dispose();
            }
            catch(Exception ex)
            {
                var a = ex.Message;
                log.Error("Error Generando el Acta de Apertura " + ex.Message);
            }
        }
        public void generarComprobanteOferta(int? idEmpresa, int? idObra, string rutaCompleta, string nombreObra)
        {
            try
            {
                PdfWriter writer = new PdfWriter(rutaCompleta);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                #region Font 
                //Creamos el tipo de Font que vamos utilizar
                //Style _emptyFont = new Style();
                //PdfFont font5 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                //_emptyFont.SetFont(font5).SetFontSize(2);

                //Style _standardFont = new Style();
                //PdfFont font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
                //_standardFont.SetFont(font).SetFontSize(10);

                //Style _standardFontChica = new Style();
                //PdfFont font2 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
                //_standardFontChica.SetFont(font2).SetFontSize(9);

                //Style _headerFont = new Style();
                //PdfFont font3 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                //_headerFont.SetFont(font3).SetFontSize(10);

                //Style _titleFont = new Style();
                //PdfFont font4 = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                //_titleFont.SetFont(font4).SetFontSize(10);

                #endregion

                ServicioDocumentacion servicioDocumentacion = new ServicioDocumentacion();
                List<RequisitoViewModels> listaArchivo = new List<RequisitoViewModels>();
                listaArchivo = servicioDocumentacion.listarRequisitoPorObraEmpresa(idEmpresa, idObra);

                #region Imagen

                string imageUrl = System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString();
                Image img = new Image(ImageDataFactory
                           .Create(imageUrl))
                           .SetTextAlignment(TextAlignment.CENTER);

                Cell celImage = new Cell();

                celImage.SetTextAlignment(TextAlignment.CENTER);
                celImage.Add(img.SetWidth(200));
                celImage.SetBorder(Border.NO_BORDER);

                #endregion

                #region Encabezado
                Table tblHeader = new Table(1);
                Cell clNombre2 = new Cell()
                    .SetTextAlignment(TextAlignment.LEFT)
                    .Add(new Paragraph("DETALLE DE OFERTA PRESENTADA POR LA OBRA: " + nombreObra + " EL " + DateTime.Now.Date.ToString("dd/MM/yyyy")))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)                    
                    //.SetBorderRight(Border.NO_BORDER)
                    //.SetBorderTop(Border.NO_BORDER)
                    //.SetBorderLeft(Border.NO_BORDER)
                    //.SetBorderRight(Border.NO_BORDER)
                    //.SetBorderTopLeftRadius(new BorderRadius(6))
                    //.SetBorderTopRightRadius(new BorderRadius(6))
                    //.SetBorderBottomLeftRadius(new BorderRadius(6))
                    //.SetBorderBottomRightRadius(new BorderRadius(6))
                    //.SetBorder(new RoundDotsBorder(ColorConstants.BLACK,1))
                    //.SetBorderBottom((new SolidBorder(ColorConstants.BLACK, 4)))                    
                    //.SetBorderRadius(new BorderRadius(6))
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    //.SetFontFamily(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN)
                    .SetFontSize(10)
                    //.SetFontFamily(StandardFonts.TIMES_BOLD)
                    .SetFontSize(10)
                    .SetHeight(30f);

                tblHeader.AddCell(clNombre2);

                /////////////////////////////////////////////////////////
                #endregion

                #region Cuerpo
                Table tblCuerpo = new Table(3);
                foreach (var item in listaArchivo)
                {
                    Cell clRequisito = new Cell()
                        .SetTextAlignment(TextAlignment.LEFT)
                        .Add(new Paragraph("Requisito: " + item.nombre))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        //.AddStyle(_standardFont)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        //.AddStyle(_standardFont)
                        //.SetFontFamily(StandardFonts.TIMES_ROMAN)
                        .SetFontSize(10)
                        .SetHeight(30f);

                    var archivo = string.Empty;
                    var fecha = string.Empty;
                    if (string.IsNullOrEmpty((item.nombreArchivo)))
                    {
                        archivo = "No presenta archivo";
                        fecha = "";
                    }
                    else
                    {
                        if(item.nombreArchivo.Contains("No se subió"))
                        {
                            archivo = "No presenta archivo";
                            fecha = "";
                        }
                        else
                        {
                            archivo = "Archivo " + item.nombreArchivo;
                            fecha = "Subido el: " + item.fechaString;
                        }
                    }

                    Cell clFile = new Cell()
                        .SetTextAlignment(TextAlignment.LEFT)
                        .Add(new Paragraph(archivo))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        //.SetFontFamily(StandardFonts.TIMES_ROMAN)
                        .SetFontSize(10)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetHeight(30f);

                    Cell clFecha = new Cell()
                        .SetTextAlignment(TextAlignment.LEFT)
                        .Add(new Paragraph(fecha))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        //.SetFontFamily(StandardFonts.TIMES_ROMAN)
                        .SetFontSize(10)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetHeight(30f);

                    tblCuerpo.AddCell(clRequisito);
                    tblCuerpo.AddCell(clFile);
                    tblCuerpo.AddCell(clFecha);
                }

                #endregion
                
                // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
                document.Add(img);
                document.Add(tblHeader);
                document.Add(tblCuerpo);
                document.Close();
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                log.Error("Error Generando el Comprobante " + ex.Message);
            }

        }
        public void generarComprobanteOfertaObser(int? idEmpresa, int? idObra, string rutaCompleta, string nombreObra)
        {
            try
            {
                PdfWriter writer = new PdfWriter(rutaCompleta);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                ServicioDocumentacion servicioDocumentacion = new ServicioDocumentacion();
                List<RequisitoViewModels> listaArchivo = new List<RequisitoViewModels>();
                listaArchivo = servicioDocumentacion.listarRequisitoPorObraEmpresaObs(idEmpresa, idObra);

                #region Imagen

                string imageUrl = System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString();
                Image img = new Image(ImageDataFactory
                           .Create(imageUrl))
                           .SetTextAlignment(TextAlignment.CENTER);

                Cell celImage = new Cell();

                celImage.SetTextAlignment(TextAlignment.CENTER);
                celImage.Add(img.SetWidth(200));
                celImage.SetBorder(Border.NO_BORDER);

                #endregion

                #region Encabezado
                Table tblHeader = new Table(1);
                Cell clNombre2 = new Cell()
                    .SetTextAlignment(TextAlignment.LEFT)
                    .Add(new Paragraph("DETALLE DE OBSERVACIONES PRESENTADA POR LA OBRA: " + nombreObra + " EL " + DateTime.Now.Date.ToString("dd/MM/yyyy")))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    //.SetBorderRight(Border.NO_BORDER)
                    //.SetBorderTop(Border.NO_BORDER)
                    //.SetBorderLeft(Border.NO_BORDER)
                    //.SetBorderRight(Border.NO_BORDER)
                    //.SetBorderTopLeftRadius(new BorderRadius(6))
                    //.SetBorderTopRightRadius(new BorderRadius(6))
                    //.SetBorderBottomLeftRadius(new BorderRadius(6))
                    //.SetBorderBottomRightRadius(new BorderRadius(6))
                    //.SetBorder(new RoundDotsBorder(ColorConstants.BLACK,1))
                    //.SetBorderBottom((new SolidBorder(ColorConstants.BLACK, 4)))                    
                    //.SetBorderRadius(new BorderRadius(6))
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    //.SetFontFamily(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN)
                    .SetFontSize(10)
                    //.SetFontFamily(StandardFonts.TIMES_BOLD)
                    .SetFontSize(10)
                    .SetHeight(30f);

                tblHeader.AddCell(clNombre2);

                /////////////////////////////////////////////////////////
                #endregion

                #region Cuerpo
                Table tblCuerpo = new Table(3);
                foreach (var item in listaArchivo)
                {
                    Cell clRequisito = new Cell()
                        .SetTextAlignment(TextAlignment.LEFT)
                        .Add(new Paragraph("Requisito: " + item.nombre))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        //.AddStyle(_standardFont)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        //.AddStyle(_standardFont)
                        //.SetFontFamily(StandardFonts.TIMES_ROMAN)
                        .SetFontSize(10)
                        .SetHeight(30f);

                    var archivo = string.Empty;
                    var fecha = string.Empty;
                    if (string.IsNullOrEmpty((item.nombreArchivo)))
                    {
                        archivo = "No presenta archivo";
                        fecha = "";
                    }
                    else
                    {
                        if (item.nombreArchivo.Contains("No se subió"))
                        {
                            archivo = "No presenta archivo";
                            fecha = "";
                        }
                        else
                        {
                            archivo = "Archivo " + item.nombreArchivo;
                            fecha = "Subido el: " + item.fechaString;
                        }
                    }

                    Cell clFile = new Cell()
                        .SetTextAlignment(TextAlignment.LEFT)
                        .Add(new Paragraph(archivo))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        //.SetFontFamily(StandardFonts.TIMES_ROMAN)
                        .SetFontSize(10)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetHeight(30f);

                    Cell clFecha = new Cell()
                        .SetTextAlignment(TextAlignment.LEFT)
                        .Add(new Paragraph(fecha))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        //.SetFontFamily(StandardFonts.TIMES_ROMAN)
                        .SetFontSize(10)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderLeft(Border.NO_BORDER)
                        .SetBorderRight(Border.NO_BORDER)
                        .SetHeight(30f);

                    tblCuerpo.AddCell(clRequisito);
                    tblCuerpo.AddCell(clFile);
                    tblCuerpo.AddCell(clFecha);
                }

                #endregion

                // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
                document.Add(img);
                document.Add(tblHeader);
                document.Add(tblCuerpo);
                document.Close();
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                log.Error("Error Generando el Comprobante de Observaciones " + ex.Message);
            }

        }
        class HeaderFooterEventHandler : IEventHandler
        {
            private PageSize pageSize;
            public void HorizontalPageEventHandler(PageSize pageSize)
            {
                this.pageSize = pageSize;
            }
            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                int pageNumber = pdfDoc.GetPageNumber(page);
                var total = pdfDoc.GetNumberOfPages();
               
                // Crear encabezado
                Paragraph header = new Paragraph("Encabezado - Página " + pageNumber)
                    .SetTextAlignment(TextAlignment.LEFT);

                string imageUrl = System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString();
                Image img = new Image(ImageDataFactory
                           .Create(imageUrl))
                           .SetTextAlignment(TextAlignment.LEFT);
                img.SetMargins(20, 30, 0, 20);
                Paragraph headeImgr = new Paragraph().Add(img);
                //headeImgr.SetFixedPosition(56, 500, 200);
                Cell celImage = new Cell();

                celImage.SetTextAlignment(TextAlignment.CENTER);
                celImage.Add(img.SetWidth(200));
                celImage.SetBorder(Border.NO_BORDER);

                float marginTop;
                if (page.GetPageSize().GetWidth() > page.GetPageSize().GetHeight())
                {
                    // Página horizontal
                    float marginHorizontalPage = 60; // Ajusta el margen superior para páginas horizontales
                    marginTop = page.GetPageSize().GetTop() - marginHorizontalPage;
                }
                else
                {
                    // Página vertical
                    float marginVerticalPage = 70; // Ajusta el margen superior para páginas verticales
                    marginTop = page.GetPageSize().GetTop() - marginVerticalPage;
                }
                float headerX = docEvent.GetPage().GetPageSize().GetLeft() + 20; // Ajusta la posición X
                float headerY = marginTop;

                // Crear pie de página
                Paragraph footer = new Paragraph("Página " + pageNumber)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFixedPosition(56, 20, 527);

                headeImgr.SetFixedPosition(headerX, headerY, 2000);
                new Canvas(page, pdfDoc.GetDefaultPageSize())
                    .Add(headeImgr)
                    .Add(footer);
            }
        }


    }
}
