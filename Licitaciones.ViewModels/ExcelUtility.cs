using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Licitaciones.ViewModels
{
    public class ExcelUtility
    {
        public ReporteExcelVM GenerarExcelGrilla(ReporteExcelVM datos, bool? nombre, bool? expediente, bool? publicacion, bool? organismo, bool? monto,
           bool? etapa, bool? fechaApertura, bool? fechaAdjudicacion, bool? entidadReceptora, bool? departamento, bool? tipoFinanciamiento, bool? plazoMeses, bool? empresasPresentadas)
        {
            IWorkbook workbook = new XSSFWorkbook();
            XSSFFont myFont = (XSSFFont)workbook.CreateFont();
            myFont.FontHeightInPoints = 11;
            myFont.FontName = "Calibri";
            XSSFCellStyle borderedCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            borderedCellStyle.SetFont(myFont);            
            borderedCellStyle.BorderLeft = BorderStyle.Thin;
            borderedCellStyle.BorderTop = BorderStyle.Thin;
            borderedCellStyle.BorderRight = BorderStyle.Thin;
            borderedCellStyle.BorderBottom = BorderStyle.Thin;
            borderedCellStyle.VerticalAlignment = VerticalAlignment.Center;
            borderedCellStyle.Alignment = HorizontalAlignment.Left;


            XSSFFont myFontTitulos = (XSSFFont)workbook.CreateFont();
            myFontTitulos.FontHeightInPoints = 12;
            myFontTitulos.FontName = "Calibri";
            myFontTitulos.Color = HSSFColor.Black.Index;
            myFontTitulos.Boldweight = (short)FontBoldWeight.Bold;
            XSSFCellStyle tituloGrilla = (XSSFCellStyle)workbook.CreateCellStyle();
            tituloGrilla.SetFont(myFontTitulos);
            tituloGrilla.BorderLeft = BorderStyle.Thin;
            tituloGrilla.BorderTop = BorderStyle.Thin;
            tituloGrilla.BorderRight = BorderStyle.Thin;
            tituloGrilla.BorderBottom = BorderStyle.Thin;
            tituloGrilla.VerticalAlignment = VerticalAlignment.Center;
            tituloGrilla.Alignment = HorizontalAlignment.Left;

            //CREAR EN LA PRIMERA FILA LOS TITULOS DE LAS COLUMNAS


            XSSFCellStyle titulo = (XSSFCellStyle)workbook.CreateCellStyle();
            titulo.SetFont(myFont);
            titulo.FillForegroundColor = IndexedColors.Lavender.Index;
            titulo.FillPattern = FillPattern.SolidForeground;
            titulo.BorderLeft = BorderStyle.Thin;
            titulo.BorderTop = BorderStyle.Thin;
            titulo.BorderRight = BorderStyle.Thin;
            titulo.BorderBottom = BorderStyle.Thin;
            titulo.VerticalAlignment = VerticalAlignment.Center;
            titulo.Alignment = HorizontalAlignment.Center;

            XSSFCellStyle TCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            TCellStyle.SetFont(myFontTitulos);     

            ISheet Sheet = workbook.CreateSheet("Report");
           
            //byte[] data = File.ReadAllBytes(System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString());
            //int pictureIndex = workbook.AddPicture(data, PictureType.PNG);
            //ICreationHelper helper = workbook.GetCreationHelper();
            //IDrawing drawing = Sheet.CreateDrawingPatriarch();
            //IClientAnchor anchor = helper.CreateClientAnchor();
            //anchor.Col1 = 0;//0 index based column
            //anchor.Row1 = 0;//0 index based row
            //IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
            //picture.Resize(1.1, 3);

            IRow Header = Sheet.CreateRow(0);
            CreateCell(Header, 0, "", TCellStyle);
            IRow Header1 = Sheet.CreateRow(1);
            CreateCell(Header1, 0, "", TCellStyle);
            IRow Header2 = Sheet.CreateRow(0);
            CreateCell(Header2, 0, "GRILLA DE OBRAS", TCellStyle);
            
            IRow HeaderRow = Sheet.CreateRow(3);
            HeaderRow.Collapsed = false;
            var i = 0;
            
            if (nombre == true)
            {
                CreateCell(HeaderRow, i, "NOMBRE", titulo);
                Sheet.SetColumnWidth(i, 9999);
                i++;
            }
            if (expediente == true)
            {
                CreateCell(HeaderRow, i, "EXPEDIENTE", titulo);
                Sheet.SetColumnWidth(i, 8000);
                i++;
            }          
            if (organismo == true)
            {
                CreateCell(HeaderRow, i, "ORGANISMO", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (monto == true)
            {
                CreateCell(HeaderRow, i, "MONTO", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (etapa == true)
            {
                CreateCell(HeaderRow, i, "ETAPA", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (publicacion == true)
            {
                CreateCell(HeaderRow, i, "FECHA PUBLICACIÓN", titulo);
                Sheet.SetColumnWidth(i, 5000);
                i++;
            }
            if (fechaApertura == true)
            {
                CreateCell(HeaderRow, i, "FECHA APERTURA", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (fechaAdjudicacion == true)
            {
                CreateCell(HeaderRow, i, "FECHA ADJUDICACIÓN", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (entidadReceptora == true)
            {
                CreateCell(HeaderRow, i, "ENTIDAD RECEPTORA", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (departamento == true)
            {
                CreateCell(HeaderRow, i, "DEPARTAMENTO", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (tipoFinanciamiento == true)
            {
                CreateCell(HeaderRow, i, "TIPO FINANCIAMIENTO", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (plazoMeses == true)
            {
                CreateCell(HeaderRow, i, "PLAZO MESES", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (empresasPresentadas == true)
            {
                CreateCell(HeaderRow, i, "EMPRESAS PRESENTADAS", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }


            int RowIndex = 4;

                foreach (var fila in datos.filas)
                {
                var y = 0;
                IRow CurrentRow = Sheet.CreateRow(RowIndex);
                if (nombre == true)
                {
                    CreateCell(CurrentRow, y, fila.nombre, borderedCellStyle);                   
                    y++;
                }
                if (expediente == true)
                {
                    CreateCell(CurrentRow, y, fila.expediente, borderedCellStyle);
                    y++;
                }               
                if (organismo == true)
                {
                    CreateCell(CurrentRow, y, fila.organismo, borderedCellStyle);
                    y++;
                }
                if (monto == true)
                {
                    CreateCell(CurrentRow, y, fila.monto, borderedCellStyle);
                    y++;
                }
                if (etapa == true)
                {
                    CreateCell(CurrentRow, y, fila.etapa, borderedCellStyle);
                    y++;
                }
                if (publicacion == true)
                {
                    CreateCell(CurrentRow, y, Convert.ToString(fila.publicacion), borderedCellStyle);
                    y++;
                }
                if (fechaApertura == true)
                {
                    CreateCell(CurrentRow, y, fila.fechaApertura, borderedCellStyle);
                    y++;
                }
                if (fechaAdjudicacion == true)
                {
                    CreateCell(CurrentRow, y, fila.fechaAdjudicacion, borderedCellStyle);
                    y++;
                }
                if (entidadReceptora == true)
                {
                    CreateCell(CurrentRow, y, fila.entidadReceptora, borderedCellStyle);
                    y++;
                }
                if (departamento == true)
                {
                    CreateCell(CurrentRow, y, fila.departamento, borderedCellStyle);
                    y++;
                }
                if (tipoFinanciamiento == true)
                {
                    CreateCell(CurrentRow, y, fila.tipoFinanciamiento, borderedCellStyle);
                    y++;
                }
                if (plazoMeses == true)
                {
                    CreateCell(CurrentRow, y, fila.plazoMeses, borderedCellStyle);
                    y++;
                }
                if (empresasPresentadas == true)
                {
                    CreateCell(CurrentRow, y, fila.empresasPresentadas, borderedCellStyle);
                    y++;
                }

                RowIndex++;

                }
            //unir celdas logo
            //CellRangeAddress union1 = new CellRangeAddress(0, 2, 0, 0);
            //Sheet.AddMergedRegion(union1);

            //unir celdas titulo
            CellRangeAddress union2 = new CellRangeAddress(0, 2, 0, (i - 1));
            Sheet.AddMergedRegion(union2);
            //centrar titulo segun tamaño
            ICellStyle mergedCellStyle = workbook.CreateCellStyle();
            mergedCellStyle.CloneStyleFrom(TCellStyle); 
            mergedCellStyle.Alignment = HorizontalAlignment.Center;             
            Sheet.GetRow(union2.FirstRow)?.Cells
                .Where(cell => cell.ColumnIndex >= union2.FirstColumn && cell.ColumnIndex <= union2.LastColumn)
                .ToList()
                .ForEach(cell => cell.CellStyle = mergedCellStyle);


            var nombreArchivo = "Grilla-" + Convert.ToDateTime(DateTime.Today).ToString("dd-MM-yyyy") + ".xlsx";
            //var path = "D:\\Archivos\\" + nombreArchivo; 
            string filePath = ConfigurationSettings.AppSettings["repositorioFiles"].ToString();
            var path = filePath + "\\" + nombreArchivo;
            using (var fileData = new FileStream(path, FileMode.Create))
            {
                workbook.Write(fileData);
                fileData.Close();
            }
            datos.filePath = path;
            datos.fileName = nombreArchivo;
            return datos;
        }        
        public ReporteExcelVM GenerarExcelGrillaEmpresa(ReporteExcelVM datos, bool? nombre, bool? expediente, bool? publicacion, bool? organismo, bool? monto, bool? etapa, bool? apertura, bool? oferta)
        {
            IWorkbook workbook = new XSSFWorkbook();
            XSSFFont myFont = (XSSFFont)workbook.CreateFont();
            myFont.FontHeightInPoints = 11;
            myFont.FontName = "Calibri";
            XSSFCellStyle borderedCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            borderedCellStyle.SetFont(myFont);
            borderedCellStyle.BorderLeft = BorderStyle.Thin;
            borderedCellStyle.BorderTop = BorderStyle.Thin;
            borderedCellStyle.BorderRight = BorderStyle.Thin;
            borderedCellStyle.BorderBottom = BorderStyle.Thin;
            borderedCellStyle.VerticalAlignment = VerticalAlignment.Center;
            borderedCellStyle.Alignment = HorizontalAlignment.Left;

            //CREAR EN LA PRIMERA FILA LOS TITULOS DE LAS COLUMNAS
            XSSFFont myFontTitulos = (XSSFFont)workbook.CreateFont();
            myFontTitulos.FontHeightInPoints = 12;
            myFontTitulos.FontName = "Calibri";
            myFontTitulos.Color = HSSFColor.Black.Index;
            myFontTitulos.Boldweight = (short)FontBoldWeight.Bold;
            XSSFCellStyle tituloGrilla = (XSSFCellStyle)workbook.CreateCellStyle();
            tituloGrilla.SetFont(myFontTitulos);
            tituloGrilla.BorderLeft = BorderStyle.Thin;
            tituloGrilla.BorderTop = BorderStyle.Thin;
            tituloGrilla.BorderRight = BorderStyle.Thin;
            tituloGrilla.BorderBottom = BorderStyle.Thin;
            tituloGrilla.VerticalAlignment = VerticalAlignment.Center;
            tituloGrilla.Alignment = HorizontalAlignment.Left;

            XSSFCellStyle titulo = (XSSFCellStyle)workbook.CreateCellStyle();
            titulo.SetFont(myFont);
            titulo.FillForegroundColor = IndexedColors.Lavender.Index;
            titulo.FillPattern = FillPattern.SolidForeground;
            titulo.BorderLeft = BorderStyle.Thin;
            titulo.BorderTop = BorderStyle.Thin;
            titulo.BorderRight = BorderStyle.Thin;
            titulo.BorderBottom = BorderStyle.Thin;
            titulo.VerticalAlignment = VerticalAlignment.Center;
            titulo.Alignment = HorizontalAlignment.Center;

            XSSFCellStyle TCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            TCellStyle.SetFont(myFontTitulos);

            ISheet Sheet = workbook.CreateSheet("Report");

            //byte[] data = File.ReadAllBytes(System.Configuration.ConfigurationSettings.AppSettings["urlLogo"].ToString());
            //int pictureIndex = workbook.AddPicture(data, PictureType.PNG);
            //ICreationHelper helper = workbook.GetCreationHelper();
            //IDrawing drawing = Sheet.CreateDrawingPatriarch();
            //IClientAnchor anchor = helper.CreateClientAnchor();
            //anchor.Col1 = 0;//0 index based column
            //anchor.Row1 = 0;//0 index based row
            //IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
            //picture.Resize(1.1, 3);

            IRow Header = Sheet.CreateRow(0);
            CreateCell(Header, 0, "", TCellStyle);
            IRow Header1 = Sheet.CreateRow(1);
            CreateCell(Header1, 0, "", TCellStyle);
            IRow Header2 = Sheet.CreateRow(0);
            CreateCell(Header2, 0, "GRILLA DE OBRAS", TCellStyle);

            IRow HeaderRow = Sheet.CreateRow(3);
            HeaderRow.Collapsed = false;
            var i = 0;

            if (nombre == true)
            {
                CreateCell(HeaderRow, i, "NOMBRE", titulo);
                Sheet.SetColumnWidth(i, 9999);
                i++;
            }
            if (expediente == true)
            {
                CreateCell(HeaderRow, i, "EXPEDIENTE", titulo);
                Sheet.SetColumnWidth(i, 8000);
                i++;
            }
            if (publicacion == true)
            {
                CreateCell(HeaderRow, i, "PUBLICACIÓN", titulo);
                Sheet.SetColumnWidth(i, 5000);
                i++;
            }
            if (organismo == true)
            {
                CreateCell(HeaderRow, i, "ORGANISMO", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (monto == true)
            {
                CreateCell(HeaderRow, i, "MONTO", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (etapa == true)
            {
                CreateCell(HeaderRow, i, "ETAPA", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (apertura == true)
            {
                CreateCell(HeaderRow, i, "APERTURA", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }
            if (oferta == true)
            {
                CreateCell(HeaderRow, i, "OFERTA", titulo);
                Sheet.SetColumnWidth(i, 6000);
                i++;
            }



            int RowIndex = 4;

            foreach (var fila in datos.filas)
            {
                var y = 0;
                IRow CurrentRow = Sheet.CreateRow(RowIndex);
                if (nombre == true)
                {
                    CreateCell(CurrentRow, y, fila.nombre, borderedCellStyle);
                    y++;
                }
                if (expediente == true)
                {
                    CreateCell(CurrentRow, y, fila.expediente, borderedCellStyle);
                    y++;
                }
                if (publicacion == true)
                {
                    CreateCell(CurrentRow, y, Convert.ToString(fila.publicacion), borderedCellStyle);
                    y++;
                }
                if (organismo == true)
                {
                    CreateCell(CurrentRow, y, fila.organismo, borderedCellStyle);
                    y++;
                }
                if (monto == true)
                {
                    CreateCell(CurrentRow, y, Convert.ToString(fila.monto), borderedCellStyle);
                    y++;
                }
                if (etapa == true)
                {
                    CreateCell(CurrentRow, y, fila.etapa, borderedCellStyle);
                    y++;
                }
                if (apertura == true)
                {
                    CreateCell(CurrentRow, y, Convert.ToString(fila.fechaApertura), borderedCellStyle);
                    y++;
                }
                if (oferta == true)
                {
                    CreateCell(CurrentRow, y, Convert.ToString(fila.estadoOferta), borderedCellStyle);
                    y++;
                }

                RowIndex++;

            }

            //unir celdas titulo
            //CellRangeAddress union1 = new CellRangeAddress(0, 2, 0, 0);
            //Sheet.AddMergedRegion(union1);

            //unir celdas titulo
            CellRangeAddress union2 = new CellRangeAddress(0, 2, 0, (i - 1));
            Sheet.AddMergedRegion(union2);
            //centrar titulo segun tamaño
            ICellStyle mergedCellStyle = workbook.CreateCellStyle();
            mergedCellStyle.CloneStyleFrom(TCellStyle);
            mergedCellStyle.Alignment = HorizontalAlignment.Center;
            Sheet.GetRow(union2.FirstRow)?.Cells
                .Where(cell => cell.ColumnIndex >= union2.FirstColumn && cell.ColumnIndex <= union2.LastColumn)
                .ToList()
                .ForEach(cell => cell.CellStyle = mergedCellStyle);

            var nombreArchivo = "Grilla-" + Convert.ToDateTime(DateTime.Today).ToString("dd-MM-yyyy") + ".xlsx";
            //var path = "D:\\Archivos\\" + nombreArchivo;
            string filePath = ConfigurationSettings.AppSettings["repositorioFiles"].ToString();
            var path = filePath + "\\" + nombreArchivo;
            using (var fileData = new FileStream(path, FileMode.Create))
            {
                workbook.Write(fileData);
                fileData.Close();
            }
            datos.filePath = path;
            datos.fileName = nombreArchivo;
            return datos;
        }
        private void CreateCell(IRow CurrentRow, int CellIndex, string Value, XSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = Style;
        }
        private void setBordersToMergedCells(ISheet sheet, IWorkbook workbook)
        {
            int numMerged = sheet.NumMergedRegions;
            for (int i = 0; i < numMerged; i++)
            {
                CellRangeAddress mergedRegions = sheet.GetMergedRegion(i);
                RegionUtil.SetBorderLeft(1, mergedRegions, sheet/*, workbook*/);
                RegionUtil.SetBorderRight(1, mergedRegions, sheet/*, workbook*/);
                RegionUtil.SetBorderTop(1, mergedRegions, sheet/*, workbook*/);
                RegionUtil.SetBorderBottom(1, mergedRegions, sheet/*, workbook*/);

            }
        }   
    }
}
