using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.IO;
using Rhino.Collections;
using System.Collections.Generic;



public enum layout_options_general
{
    position,
    paper,
    scale,
    name,
    edge,
    custom_ttributes
}

public enum layout_options_position
{
    from_point,
    from_rectangle
}

public enum layout_options_paper
{
    iso_paper_sizes,
    custom_paper_sizes
}

public enum layout_options_name
{
    layout_name,
    drawing_name,
    drawing_alternative_name
}

public enum layout_options_edge
{
    drawing_edge_mm,
    printing_edge_mm
}

public enum justification
{
    bottom_left,
    bottom_right,
    top_left,
    top_right,
    center
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

            Boolean choosing_layout_options = true;

            // ask for options

            RPHLayoutSettings settings = new RPHLayoutSettings();

            while (choosing_layout_options)
            {

                List<string> layout_options_general = new List<string> { "Position", "Paper", "Scale", "Name", "Edge", "User_Attributes" };
                List<string> layout_options_general_defaults = new List<string> { "Origin", "A3", "1:100", "Default", "Default", "Default_User_Attributes" };

                LayoutOptionDialog layout_options_dialog_general = new LayoutOptionDialog("Layout Settings:", layout_options_general, layout_options_general_defaults);
                
                
                
                int choice_index = layout_options_dialog_general.GetChoiceIndex();
                

                switch(choice_index)
                {
                    case 1:
                        List<string> layout_options_position = new List<string> { "From_Point", "From_Rectangle" };
                        LayoutOptionDialog layout_options_dialog_position = new LayoutOptionDialog("Specify Layout Position:", layout_options_position);
                        int choice_jndex = layout_options_dialog_position.GetChoiceIndex();
                        switch(choice_jndex)
                        {
                            case 1:
                                GetPoint get_point = new GetPoint();
                                get_point.SetCommandPrompt("Specify a Point");s
                                get_point.AddOptionEnumList("Justification", justification.bottom_right);
                                int i = -999;
                                while (true)
                                {
                                    get_point.Get();
                                    
                                    //fixthis
                                    if (i) )
                                    {
                                        Point3d pt = get_point.Point();
                                        settings.SetLayoutOrigin(pt);
                                        
                                        break;
                                        
                                    }
                                    

                                }
                                break;

                            case 2:
                                GetObject get_object = new GetObject();
                                get_object.SetCommandPrompt("Specify a Rectangle");
                                get_object.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
                                
                                get_object.Get();
                                   
                                break;

                            default:
                                break;

                        }




                        break;

                    case 2:
                        break;
                        
      

                }

                // fix
                //layout_options.AddOptionEnumList("General Layout Options:", );

                

                //GetResult result = layout_options.Get();
                //string msg = layout_options.OptionIndex().ToString();

                Rhino.UI.SaveFileDialog save = new Rhino.UI.SaveFileDialog();
                //save.Title = choice_index.ToString();
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
        // paper settings
        public string paper_size_name;
        public double paper_width;
        public double paper_height;
        public bool is_landscape;

        // plotting settings
        public double scale;
        public bool drawing_elements_scaling;

        // layout settings
        public int layout_id;
        public string layout_name;
        public Point3d layout_origin;
        public Plane layout_plane;
        public double model_width;
        public double model_height;
        public Rectangle3d layout_boundingbox;

        // drawing settings
        public string drawing_name;
        public string drawing_alt_name;
        public double printing_edge;
        public double drawing_edge;

        // attribute settings
        public ExtendedArchivableDictionary user_dictionary = new ExtendedArchivableDictionary();

        // private variables
        // private Vector3d unit_x = new Vector3d(1, 0, 0);
        private Vector3d layout_x = new Vector3d(1, 0, 0);

        public RPHLayoutSettings()
        {
            // default settings
            this.paper_size_name = "A3";
            this.paper_width = 420;
            this.paper_height = 297;
            UpdatePageOrientation();
            UpdateModelDimensions();

            this.scale = 100;
            this.drawing_elements_scaling = true;

            this.layout_id = 0;
            this.layout_name = "Default Layout";
            this.layout_origin = new Point3d(0, 0, 0);
            UpdateLayoutPlane();
            UpdateLayoutBoundingBox();

            this.drawing_name = "Default Drawing";
            this.drawing_alt_name = "Default Drawing Alt Name";
            this.printing_edge = 3;
            this.drawing_edge = 10;
            
        }

        public void SetPaperSize(int code)
        {
            switch (code)
            {
                case 0:
                    this.paper_size_name = "A0";
                    this.paper_width = 1189;
                    this.paper_height = 841;
                    break;

                case 1:
                    this.paper_size_name = "A1";
                    this.paper_width = 841;
                    this.paper_height = 594;
                    break;

                case 2:
                    this.paper_size_name = "A2";
                    this.paper_width = 594;
                    this.paper_height = 420;
                    break;

                case 3:
                    this.paper_size_name = "A3";
                    this.paper_width = 420;
                    this.paper_height = 297;
                    break;

                case 4:
                    this.paper_size_name = "A4";
                    this.paper_width = 297;
                    this.paper_height = 210;
                    break;

                case 5:
                    this.paper_size_name = "A5";
                    this.paper_width = 210;
                    this.paper_height = 148;
                    break;

                default:
                    this.paper_size_name = "A3";
                    this.paper_width = 420;
                    this.paper_height = 297;
                    break;

            }
            UpdateLocalVariables();
        }

        public void SetPaperSize(double width, double height)
        {
            this.paper_width = width;
            this.paper_height = height;
            this.paper_size_name = string.Format("Page with width {0} mm, height {1} mm", this.paper_width, this.paper_height);
            UpdateLocalVariables();
        }

        public void SetPaperOrientation(bool is_landscape)
        {
            UpdatePageOrientation();
            if (this.is_landscape != is_landscape)
            {
                double temp_w = this.paper_width;
                this.paper_height = this.paper_width;
                this.paper_width = temp_w;
                UpdateLocalVariables();
            } else
            {
                UpdateLocalVariables();
            }
            return;
        }

        public void SetScale(double scale)
        {
            this.scale = scale;
            UpdateLocalVariables();
        }

        public void SetDrawingElementsScaling(bool scaling)
        {
            this.drawing_elements_scaling = scaling;
            UpdateLocalVariables();
        }

        public void SetLayoutId(int id)
        {
            this.layout_id = id;
            UpdateLocalVariables();
        }

        public void SetLayoutName(string name)
        {
            this.layout_name = name;
            UpdateLocalVariables();
        }

        public void SetLayoutOrigin(Point3d origin)
        {
            this.layout_origin = origin;
            UpdateLocalVariables();
        }

        public void SetLayoutPlane(Point3d origin)
        {
            this.layout_origin = origin;
            UpdateLocalVariables();
        }

        public void SetLayoutPlane(Point3d origin, Vector3d x_axis)
        {
            this.layout_origin = origin;
            this.layout_x = x_axis;
            UpdateLocalVariables();
        }

        public void SetLayoutPlane(Vector3d x_axis)
        {
            this.layout_x = x_axis;
            UpdateLocalVariables();
        }

        public void SetLayoutBoundingBox(Rectangle3d rectangle)
        {
            this.layout_boundingbox = rectangle;
            this.model_width = Math.Abs(rectangle.X.T1 - rectangle.X.T0);
            this.model_height = Math.Abs(rectangle.Y.T1 - rectangle.Y.T0);
            this.paper_width = this.model_width / this.scale * 1000;
            this.paper_height = this.model_height / this.scale * 1000;
            this.layout_origin = rectangle.PointAt(0);
            this.layout_x = new Vector3d(rectangle.PointAt(1) - rectangle.PointAt(0));
            this.SetLayoutPlane(this.layout_origin, this.layout_x);
            UpdateLocalVariables();
        }

        public void SetLayoutBoundingBox(double width, double height)
        {
            this.model_width = width;
            this.model_height = height;
            this.paper_width = this.model_width / this.scale * 1000;
            this.paper_height = this.model_height / this.scale * 1000;
            UpdateLocalVariables();
        }

        public void SetDrawingName(string name)
        {
            this.drawing_name = name;
            UpdateLocalVariables();
        }

        public void SetDrawingAltName(string name)
        {
            this.drawing_alt_name = name;
            UpdateLocalVariables();
        }

        public void SetPrintingEdge(double edge)
        {
            this.printing_edge = edge;
            UpdateLocalVariables();
        }

        public void SetDrawingEdge(double edge)
        {
            this.drawing_edge = edge;
            UpdateLocalVariables();
        }

        private void UpdateLocalVariables()
        {
            /// use private methods to update all relevant local variables IN ORDER -> UPDATE EVERYTHING!

            // a list of methods to implement

            UpdatePageOrientation();
            UpdateLayoutPlane();
            UpdateModelDimensions();
            UpdateLayoutBoundingBox();
            UpdateUserDictionary();

            return;
        }

        private void UpdatePageOrientation() 
        {
            /// private method called on every change made to change the page size
            if (this.paper_width >= this.paper_height)
            {
                this.is_landscape = true;
                return;
            } else
            {
                this.is_landscape = false;
                return;
            }
        }

        private void UpdateLayoutPlane()
        {
            this.layout_plane = new Plane(this.layout_origin, this.layout_x);
        }

        private void UpdateModelDimensions()
        {
            this.model_width = this.paper_width * this.scale / 1000;
            this.model_height = this.paper_height * this.scale / 1000;
        }

        private void UpdateLayoutBoundingBox()
        {
            this.layout_boundingbox = new Rectangle3d(this.layout_plane, this.model_width, this.model_height);   
        }

        private void UpdateUserDictionary()
        {
            user_dictionary.SetLayoutSetting(this);
            
        }


        public void DrawLayoutBoundingBox(RhinoDoc doc)
        {
            doc.Objects.AddRectangle(this.layout_boundingbox);
        }

    }

    public class ExtendedArchivableDictionary : ArchivableDictionary
    {
        /// <summary>
        /// Extended ArchivableDictionary class with new methods to set and get RPHLayoutSettings layout_settings
        /// </summary>
        private RPHLayoutSettings layout_settings;

        public void SetLayoutSetting(RPHLayoutSettings layout_settings)
        {          
            this.layout_settings = layout_settings;
        }
        
        public RPHLayoutSettings GetLayoutSetting()
        {
            return this.layout_settings;
        }
    }

    public class LayoutOptionDialog
    {
        private GetOption option = new Rhino.Input.Custom.GetOption();

        private List<string> option_names;
        private List<string> option_defaults;
        private List<int> option_indices = new List<int>();
        private string msg;

        private int choice_index;

        public LayoutOptionDialog(string msg, List<string> option_names, List<string> option_defaults)
        {
            this.msg = msg;
            this.option_names = option_names;
            this.option_defaults = option_defaults;

            this.option.SetCommandPrompt(msg);
            for (int i = 0; i < this.option_names.Count; i++)
            {
                this.option.AddOption(this.option_names[i], this.option_defaults[i]);
                this.option_indices.Add(i);
            }
            this.option.Get();
            this.choice_index = this.option.Option().Index;
        }

        public LayoutOptionDialog(string msg, List<string> option_names)
        {
            this.msg = msg;
            this.option_names = option_names;

            this.option.SetCommandPrompt(msg);
            for (int i = 0; i < this.option_names.Count; i++)
            {
                this.option.AddOption(this.option_names[i]);
                this.option_indices.Add(i);
            }
            this.option.Get();
            this.choice_index = this.option.Option().Index;
        }

        public int GetChoiceIndex()
        {
            return this.choice_index;
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