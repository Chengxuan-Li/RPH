using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.IO;
using Rhino.Collections;
using System.Collections.Generic;


namespace RPH
{
    public class RPHPrint : Command
    {
        static RPHPrint _instance;
        public RPHPrint()
        {
            _instance = this;
        }

        ///<summary>The only instance of the RPHPrint command.</summary>
        public static RPHPrint Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RPHPrint"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //set the default output path


            String[] path_sep = new String[1] { ".3dm" };
            String full_path = Rhino.ApplicationSettings.FileSettings.WorkingFolder;
            if (full_path != null)
            {
                String path = full_path.Split(path_sep, StringSplitOptions.None)[0];
                String msg = String.Format("Current output directory is: {0}, do you wish a change???", path);

                //GetOption get_dir = new GetOption();
                Boolean change_path = false;
                RhinoGet.GetBool(msg, true, "NO", "YES", ref change_path);

                if (change_path) {
                    var opnDlg = new Rhino.UI.OpenFileDialog();//ANY dialog

                        //opnDlg.Filter = "Png Files (*.png)|*.png";
                        //opnDlg.Filter = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv"
                        
                    if (opnDlg.ShowOpenDialog());
                    {
                        //opnDlg.SelectedPath -- your result
                    }
                    
                }

                //printing

                double original_linetypescale = doc.Linetypes.LinetypeScale;
                String file_name = String.Concat(path, @"\20220118TESTPRINTDOC.pdf");
                //String file_name = @"C:\Users\liche\Dropbox\PC\Desktop\EX12\Brief 04\Detail\Original\33.pdf";

                Rhino.Display.RhinoPageView[] pages = doc.Views.GetPageViews();
                Rhino.FileIO.FilePdf pdf = Rhino.FileIO.FilePdf.Create();
                double dpi = 600;

                // linetype issue!!!!!!!!!!

                foreach (Rhino.Display.RhinoPageView page in pages)
                {
                    Rhino.DocObjects.DetailViewObject first_detail = page.GetDetailViews()[0];
                    double page_scale = first_detail.Attributes.UserDictionary.GetDouble("page_scale");
                    double linetypescale_multiplier = page_scale * 10;
                    double print_linetypescale = linetypescale_multiplier * original_linetypescale;
                    doc.Linetypes.LinetypeScale = print_linetypescale;

                    Rhino.Display.ViewCaptureSettings captured_page = new Rhino.Display.ViewCaptureSettings(page, dpi);
                    pdf.AddPage(captured_page);
                }

                pdf.Write(file_name);
                doc.Linetypes.LinetypeScale = original_linetypescale;
                

            }


            
            return Result.Success;
        }
    }
}

/*


Rhino.UI.SaveFileDialog save = new Rhino.UI.SaveFileDialog();
save.Title = ?
save.Filter = ".pdf";
save.ShowSaveDialog();

*/