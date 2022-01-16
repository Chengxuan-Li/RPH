using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.IO;
using Rhino.Collections;
using System.Collections.Generic;



public enum selection
{
    sel1,
    sel2,
    selotr
}

namespace RPH
{
    public class RPHAddNewLayout : Command
    {
        static RPHAddNewLayout _instance;
        public RPHAddNewLayout()
        {
            _instance = this;
        }

        ///<summary>The only instance of the RPHAddNewLayout command.</summary>
        public static RPHAddNewLayout Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "RPHAddNewLayout"; }
        }


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            Rhino.Display.RhinoPageView[] page_views = doc.Views.GetPageViews();

            ArchivableDictionary layout_info = new ArchivableDictionary();

            Boolean choosing_layout_options = true;

            // ask for options


            RPHLayoutSettings settings = new RPHLayoutSettings("test_string");

            while (choosing_layout_options)
            {
                var layout_options = new Rhino.Input.Custom.GetOption();


                

                layout_options.AddOptionEnumList(settings.GetId(), selection.sel1);
                
                layout_options.SetCommandPrompt("abcd");

                GetResult result = layout_options.Get();
                string msg = layout_options.OptionIndex().ToString();

                Rhino.UI.SaveFileDialog save = new Rhino.UI.SaveFileDialog();
                save.Title = msg;
                save.Filter = ".pdf";
                save.ShowSaveDialog();

                
                // ask for page size




                // ask for position



                // ask for justification
                choosing_layout_options = false;
            }














            // check whether all existing layouts are created with RPH

            // return messages if not all layouts are created with RPH

            // add new layout

            // add new layout attributes

            // add detail view

            // adjust detail view position and scale

            return Result.Success;
        }
    }
    public class RPHLayoutSettings
    {   
        private string txt;
        public RPHLayoutSettings(string txtt)
        {
            this.txt = txtt;
        }
        
        public string GetId()
        {
            return this.txt;
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