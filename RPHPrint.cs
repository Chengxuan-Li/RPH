using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.IO;
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
                String file_name = String.Concat(path, @"\20220113K.pdf");
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
    double startX = 0.0;
    ArchivableDictionary layout_info = new ArchivableDictionary();
    RhinoDoc doc = RhinoDoc.ActiveDoc;
    doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;
    var page_views = doc.Views.GetPageViews();
    int page_number = (page_views == null) ? 1 : page_views.Length + 1;

    if (_RunIt != true)
    {
      return;
    }

    layout_info.Set("page_number", page_number);

    var pageview = doc.Views.AddPageView(string.Format("L{0}", page_number), extentX, extentY);
    layout_info.Set("page_extent_X", extentX);
    layout_info.Set("page_extent_Y", extentY);
    layout_info.Set("detail_extent_X", drawingX);
    layout_info.Set("detail_extent_Y", drawingY);
    layout_info.Set("page_scale", scale);

    for (int i = 1; i < page_number; i++)
    {
      Point3d origin = new Point3d(0, 0, 0);
      Vector3d vector = new Vector3d(1, 0, 0);
      Plane frame = new Plane(origin, vector);
      var detail_viewport = page_views[i - 1].GetDetailViews()[0].Viewport;
      detail_viewport.GetCameraFrame(out frame);
      var box = detail_viewport.GetFrustumBoundingBox();
      startX = box.GetCorners()[1].X;
    }
    layout_info.Set("page_bottom_left_X", startX);
    if( pageview != null )
    {
      Rhino.Geometry.Point2d top_left = new Rhino.Geometry.Point2d(0, extentY);
      Rhino.Geometry.Point2d bottom_right = new Rhino.Geometry.Point2d(extentX, 0);
      var detail = pageview.AddDetailView("ModelView", top_left, bottom_right, Rhino.Display.DefinedViewportProjection.Top);

      if (detail != null)
      {
        pageview.SetActiveDetail(detail.Id);
        Rhino.Geometry.Point3d center = new Rhino.Geometry.Point3d(0.5 * drawingX + startX, 0.5 * drawingY, 0);
        detail.Viewport.SetCameraLocation(center, true);
        detail.Viewport.SetCameraTarget(center, true);
        detail.CommitViewportChanges();
        detail.DetailGeometry.IsProjectionLocked = true;
        detail.DetailGeometry.SetScale(scale, doc.ModelUnitSystem, 1, doc.PageUnitSystem);
        // Commit changes
        // Append Rhino.ArchivableDictionary layout_info to Rhino.DocObjects.DetailViewObject.Attributes.UserDictionary
        detail.Attributes.UserDictionary.AddContentsFrom(layout_info);
        detail.CommitChanges();
        Print(detail.Attributes.UserDictionary.ToString());

      }
      pageview.SetPageAsActive();
      doc.Views.ActiveView = pageview;
      doc.Views.Redraw();
      NumSheet = page_number;
      Size = drawingX;
      return;
    }
    return;

*/