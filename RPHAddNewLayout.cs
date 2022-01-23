using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.Display;
using Rhino.DocObjects.Tables;
using System;
using System.IO;
using Rhino.Collections;
using System.Collections.Generic;
using System.Drawing;

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
            bool choosing_layout_options = true;
            bool commit_layout_creation = true;
            // ask for options

            RPHLayoutSettings settings = new RPHLayoutSettings();

            while (choosing_layout_options)
            {
                settings.LayoutCreationPreview(true);

                List<string> layout_options_general = new List<string> { "Position", "Paper", "Scale", "Name", "Edge", "User_Attributes" };
                List<string> layout_options_general_defaults = new List<string> { "", settings.paper_size_name, String.Format("1:{0}", settings.scale), "", "", "" };

                List<string> layout_options_position = new List<string> { "From_Point", "From_Content_Objects" };
                List<string> justification = new List<string> { "Bottom_Left", "Bottom_Right", "Top_Left", "Top_Right", "Center" };

                List<string> layout_options_paper = new List<string> { "ISO_Paper_Code", "Orientation", "Use_Custom_Paper_Size" };
                List<string> layout_options_paper_defaults = new List<string> { settings.paper_size_name, "Portrait", "No" };
                if (settings.is_landscape)
                {
                    layout_options_paper_defaults = new List<string> { settings.paper_size_name, "Landscape", "No" };
                }

                List<string> layout_options_paper_codes = new List<string> { "A0", "A1", "A2", "A3", "A4", "A5" };

                List<string> layout_options_name = new List<string> { "layout_name", "drawing_name", "drawing_alt_name" };
                List<string> layout_options_name_defaults = new List<string> { settings.layout_name, settings.drawing_name, settings.drawing_alt_name };

                Point3d point = new Point3d(0, 0, 0);
                int bounding_box_mode = 2;






                GetOption options_dialog_general = new GetOption();
                LayoutOptionDialog layout_options_dialog_general = new LayoutOptionDialog(options_dialog_general, "Layout Settings:", layout_options_general, layout_options_general_defaults);
                int general_choice_index = layout_options_dialog_general.DialogResult().Getint("choice_index", -1);

                
                switch(general_choice_index)
                {
                    case 1:
                        /// position options
                        GetOption options_dialog_position = new GetOption();
                        LayoutOptionDialog layout_options_dialog_position = new LayoutOptionDialog(options_dialog_position, "Specify Layout Position:", layout_options_position);
                        int position_choice_index = layout_options_dialog_position.DialogResult().Getint("choice_index", -1);

                        switch(position_choice_index)
                        {
                            case 1:
                                bool choosing_point = true;
                                int justification_choice_index = 1;
                                
                                while (choosing_point)
                                {
                                    GetPoint options_dialog_point = new GetPoint();
                                    Vector3d[] bounding_box_relative_transform_vectors = settings.GetBoundingBoxRelativeRelativeTransformVectors(justification_choice_index);

                                    options_dialog_point.DynamicDraw += (sender, e) => e.Display.DrawLine(
                                        e.CurrentPoint + bounding_box_relative_transform_vectors[0], e.CurrentPoint + bounding_box_relative_transform_vectors[1],
                                        Color.White, 2);

                                    options_dialog_point.DynamicDraw += (sender, e) => e.Display.DrawLine(
                                        e.CurrentPoint + bounding_box_relative_transform_vectors[1], e.CurrentPoint + bounding_box_relative_transform_vectors[2],
                                        Color.White, 2);

                                    options_dialog_point.DynamicDraw += (sender, e) => e.Display.DrawLine(
                                        e.CurrentPoint + bounding_box_relative_transform_vectors[2], e.CurrentPoint + bounding_box_relative_transform_vectors[3],
                                        Color.White, 2);

                                    options_dialog_point.DynamicDraw += (sender, e) => e.Display.DrawLine(
                                        e.CurrentPoint + bounding_box_relative_transform_vectors[3], e.CurrentPoint + bounding_box_relative_transform_vectors[0],
                                        Color.White, 2);

                                    LayoutOptionDialog layout_options_dialog_point = new LayoutOptionDialog(options_dialog_point, "Specify a Point", new List<string> { "Justification" }, new List<string> {justification[justification_choice_index - 1]});
                                    int point_choice_index = layout_options_dialog_position.DialogResult().Getint("choice_index", -1);
                                    

                                    if (layout_options_dialog_point.DialogResult().Getint("choice_index", -1) == 0)
                                    {
                                        GetOption option_dialog_justification = new GetOption();
                                        
                                        LayoutOptionDialog layout_options_dialog_justification = new LayoutOptionDialog(option_dialog_justification, "Specify Justification Method", justification);
                                        justification_choice_index = layout_options_dialog_justification.DialogResult().Getint("choice_index", 1);
                                    }

                                    if (layout_options_dialog_point.DialogResult().Getint("choice_index", -1) == 1)
                                    {
                                        point = layout_options_dialog_point.DialogResult().GetPoint3d("result", new Point3d(0, 0, 0));
                                        break;
                                    }

                                    if (layout_options_dialog_point.DialogResult().Getint("choice_index", -1) == -1)
                                    {
                                        break;
                                    }
                                }
                                Rectangle3d layout_rectangle;
                                switch (justification_choice_index)
                                {
                                    case 1:
                                        layout_rectangle = settings.layout_boundingbox;
                                        layout_rectangle.Transform(Transform.Translation(point - layout_rectangle.PointAt(4)));
                                        settings.SetLayoutScaleOrigin(point);
                                        settings.SetLayoutBoundingBox(layout_rectangle);
                                        settings.SetLayoutScaleOrigin(point);
                                        break;
                                    case 2:
                                        layout_rectangle = settings.layout_boundingbox;
                                        layout_rectangle.Transform(Transform.Translation(point - layout_rectangle.PointAt(1)));
                                        settings.SetLayoutScaleOrigin(point);
                                        settings.SetLayoutBoundingBox(layout_rectangle);
                                        settings.SetLayoutScaleOrigin(point);
                                        break;
                                    case 3:
                                        layout_rectangle = settings.layout_boundingbox;
                                        layout_rectangle.Transform(Transform.Translation(point - layout_rectangle.PointAt(3)));
                                        settings.SetLayoutScaleOrigin(point);
                                        settings.SetLayoutBoundingBox(layout_rectangle);
                                        settings.SetLayoutScaleOrigin(point);
                                        break;
                                    case 4:
                                        layout_rectangle = settings.layout_boundingbox;
                                        layout_rectangle.Transform(Transform.Translation(point - layout_rectangle.PointAt(2)));
                                        settings.SetLayoutScaleOrigin(point);
                                        settings.SetLayoutBoundingBox(layout_rectangle);
                                        settings.SetLayoutScaleOrigin(point);
                                        break;
                                    case 5:
                                        layout_rectangle = settings.layout_boundingbox;
                                        layout_rectangle.Transform(Transform.Translation(point - layout_rectangle.Center));
                                        settings.SetLayoutScaleOrigin(point);
                                        settings.SetLayoutBoundingBox(layout_rectangle);
                                        settings.SetLayoutScaleOrigin(point);
                                        break;
                                    default:

                                        break;
                                }

                                settings.LayoutCreationPreview(true);
                                doc.Views.Redraw();
                                break;

                            case 2:
                                bool choosing_content_objects = true;

                                settings.LayoutCreationPreview(false);

                                GetObject options_dialog_objects = new GetObject();
                                options_dialog_objects.GeometryFilter = Rhino.DocObjects.ObjectType.Point | Rhino.DocObjects.ObjectType.Curve | Rhino.DocObjects.ObjectType.PointSet;

                                LayoutOptionDialog layout_options_dialog_objects = new LayoutOptionDialog(options_dialog_objects, "Pick Content Objects"
                                    /*, new List<string> { "Bounding_Box_Mode" }*/
                                    );
                                int content_objects_choice_index = layout_options_dialog_objects.DialogResult().Getint("choice_index", -1);
                                /*
                                if (content_objects_choice_index == 0)
                                {
                                    GetOption options_dialog_bounding_box_mode = new GetOption();
                                    LayoutOptionDialog layout_options_dialog_bounding_box_mode = new LayoutOptionDialog(options_dialog_bounding_box_mode, "Specify Bounding Box Mode", new List<string> { "Accurate", "Coarse" });
                                    if (layout_options_dialog_bounding_box_mode.DialogResult().Getint("choice_index", -1) != -1)
                                    {
                                        bounding_box_mode = layout_options_dialog_bounding_box_mode.DialogResult().Getint("choice_index", -1);
                                    }
                                }
                                */

                                bool accurate = false;

                                ObjectListBoundingBox Box = new ObjectListBoundingBox(options_dialog_objects.Objects(), accurate);
                                BoundingBox box = Box.Box();
                                Rectangle3d rectangle = new Rectangle3d(Plane.WorldXY, box.Min, box.Max);

                                settings.SetLayoutBoundingBox(rectangle);
                                settings.SetLayoutScaleOrigin(rectangle.Center);

                                settings.LayoutCreationPreview(true);
                                doc.Views.Redraw();


                                break;

                            default:
                                break;

                        }
                        break;

                    case 2:
                        settings.LayoutCreationPreview(true);
                        doc.Views.Redraw();

                        /// paper options
                        bool choosing_paper = true;
                        while (choosing_paper)
                        {
                            layout_options_paper_defaults[0] = settings.paper_size_name;
                            if (settings.is_landscape)
                            {
                                layout_options_paper_defaults[1] = "Landscape";
                            } else
                            {
                                layout_options_paper_defaults[1] = "Portrait";
                            }

                            GetOption options_dialog_paper = new GetOption();
                            LayoutOptionDialog layout_options_dialog_paper = new LayoutOptionDialog(options_dialog_paper, "Paper Settings:", layout_options_paper, layout_options_paper_defaults);
                            int paper_choice_index = layout_options_dialog_paper.DialogResult().Getint("choice_index", -1);

                            if (paper_choice_index == -1)
                            {
                                choosing_paper = false;
                            }

                            switch (paper_choice_index)
                            {
                                case 1:

                                    GetOption options_dialog_paper_codes = new GetOption();
                                    LayoutOptionDialog layout_options_dialog_paper_codes = new LayoutOptionDialog(options_dialog_paper_codes, "ISO Paper Size:", layout_options_paper_codes);
                                    int paper_code_choice_index = layout_options_dialog_paper_codes.DialogResult().Getint("choice_index", -1);
                                    if (settings.is_landscape)
                                    {
                                        if (paper_code_choice_index != -1)
                                        {
                                            settings.SetPaperSize(paper_code_choice_index - 1);
                                        }
                                    } else
                                    {
                                        if (paper_code_choice_index != -1)
                                        {
                                            settings.SetPaperSize(paper_code_choice_index - 1);
                                            settings.SetPaperSize(paper_code_choice_index - 1);
                                        }
                                        settings.SetPaperOrientation(false);//remain original orientation because SetPaperSize() always sets it into landscape XD NOOB ME
                                    }
                                    doc.Views.Redraw();
                                    break;
                                case 2:
                                    GetOption options_dialog_paper_orientation = new GetOption();
                                    LayoutOptionDialog layout_options_dialog_paper_orientation = new LayoutOptionDialog(options_dialog_paper_orientation, "Orientation:", new List<string> { "Landscape", "Portrait" });
                                    int paper_orientation_choice_index = layout_options_dialog_paper_orientation.DialogResult().Getint("choice_index", -1);
                                    if (paper_orientation_choice_index == 1)
                                    {
                                        settings.SetPaperOrientation(true);
                                    }
                                    if (paper_orientation_choice_index == 2)
                                    {
                                        settings.SetPaperOrientation(false);
                                    }
                                    doc.Views.Redraw();
                                    break;
                                case 3:
                                    double paper_width = settings.paper_width;
                                    double paper_height = settings.paper_height;

                                    GetNumber options_dialog_paper_width = new GetNumber();
                                    LayoutOptionDialog layout_options_dialog_paper_width = new LayoutOptionDialog(options_dialog_paper_width, "Width (mm)");

                                    int paper_width_choice_index = layout_options_dialog_paper_width.DialogResult().Getint("choice_index", -1);
                                    if (paper_width_choice_index == 1)
                                    {
                                        paper_width = layout_options_dialog_paper_width.DialogResult().GetDouble("result");
                                    }

                                    GetNumber options_dialog_paper_height = new GetNumber();
                                    LayoutOptionDialog layout_options_dialog_paper_height = new LayoutOptionDialog(options_dialog_paper_height, "Height (mm)");

                                    int paper_height_choice_index = layout_options_dialog_paper_height.DialogResult().Getint("choice_index", -1);
                                    if (paper_height_choice_index == 1)
                                    {
                                        paper_height = layout_options_dialog_paper_height.DialogResult().GetDouble("result");
                                    }

                                    settings.SetPaperSize(paper_width, paper_height);
                                    doc.Views.Redraw();
                                    layout_options_paper_defaults[2] = "Yes";
                                    break;
                                default:

                                    break;
                            }

                        }

                        break;

                    case 3:
                        settings.LayoutCreationPreview(true);
                        doc.Views.Redraw();
                        /// scale options
                        double scale = settings.scale;
                        GetNumber options_dialog_scale = new GetNumber();
                        LayoutOptionDialog layout_options_dialog_scale = new LayoutOptionDialog(options_dialog_scale, "Specify Drawing Scale (e.g. 100 means 1:100)");
                        int paper_scale_choice_index = layout_options_dialog_scale.DialogResult().Getint("choice_index", -1);
                        if (paper_scale_choice_index == 1)
                        {
                            scale = layout_options_dialog_scale.DialogResult().GetDouble("result", 100);
                        }
                        settings.SetScale(scale);
                        doc.Views.Redraw();
                        break;

                    case 4:
                        settings.LayoutCreationPreview(true);
                        /// name options


                        doc.Views.Redraw();
                        break;

                    case 5:
                        settings.LayoutCreationPreview(true);
                        /// edge options

                        doc.Views.Redraw();
                        break;


                    case -1:
                        choosing_layout_options = false;
                        settings.LayoutCreationPreview(false);
                        doc.Views.Redraw();
                        break;

                    default:
                        choosing_layout_options = false;
                        doc.Views.Redraw();
                        break;
                }


            }
            settings.LayoutCreationPreview(true);
            doc.Views.Redraw();
            GetOption options_dialog_commit = new GetOption();
            options_dialog_commit.AcceptEnterWhenDone(true);
            options_dialog_commit.AcceptNothing(true);
            LayoutOptionDialog layout_options_dialog_commit = new LayoutOptionDialog(options_dialog_commit, "Previewing ... Commit Changes?", new List<string> { "Yes", "No" });
            if (layout_options_dialog_commit.DialogResult().Getint("choice_index", -1) > 1 | layout_options_dialog_commit.DialogResult().Getint("choice_index", -1) < 0)
            {
                settings.LayoutCreationPreview(false);
                doc.Views.Redraw();
                return Result.Cancel;

            }
            else
            {
                commit_layout_creation = true;
                choosing_layout_options = false;
            }

            if (commit_layout_creation)
            {
                settings.AddLayout(doc);
            }

            settings.LayoutCreationPreview(false);
            doc.Views.Redraw();


            // check whether all existing layouts are created with RPH

            // return messages if not all layouts are created with RPH

            // add new layout

            // add new layout attributes

            // add detail view

            // adjust detail view position and scale

            return Result.Success;
        }
    }
    public class RPHLayoutSettings : ICloneable
    {
        // paper settings
        public string paper_size_name;
        public double paper_width;
        public double paper_height;
        public bool is_landscape;

        // plotting settings
        public double scale = 100;
        public bool drawing_elements_scaling;

        // layout settings
        public int layout_id;
        public Guid layout_guid;
        public string layout_name;
        public Point3d layout_origin;
        public Point3d layout_scale_origin;
        public Plane layout_plane;
        public Plane layout_scale_plane;
        public double model_width;
        public double model_height;
        public Rectangle3d layout_boundingbox;

        // drawing settings
        public string drawing_name;
        public string drawing_alt_name;
        public double printing_edge;
        public double drawing_edge;

        // attribute settings
        public ExtendedArchivableDictionary extended_user_dictionary = new ExtendedArchivableDictionary();
        public ArchivableDictionary user_dictionary = new ArchivableDictionary();


        // preview settings
        DisplayConduitLayoutPreview layout_preview_display_conduit = new DisplayConduitLayoutPreview();



        // private variables
        // private Vector3d unit_x = new Vector3d(1, 0, 0);
        private Vector3d layout_x = new Vector3d(0, 0, 1);

        public RPHLayoutSettings()
        {
            this.paper_size_name = "A3";
            this.paper_width = 420;
            this.paper_height = 297;
            UpdatePageOrientation();
            UpdateModelDimensions();

            this.scale = 100;
            this.drawing_elements_scaling = true;

            this.layout_id = 0;
            this.layout_guid = Guid.NewGuid();
            this.layout_name = "L_" + this.layout_guid.ToString().Substring(0, 4);
            this.layout_origin = new Point3d(0, 0, 0);
            this.layout_scale_origin = new Point3d(0, 0, 0);
            UpdateLayoutPlane();
            this.layout_boundingbox = new Rectangle3d(this.layout_plane, this.model_width, this.model_height);
            UpdateLayoutBoundingBox();

            this.drawing_name = "Default Drawing";
            this.drawing_alt_name = "Default Drawing Alt Name";
            this.printing_edge = 3;
            this.drawing_edge = 10;
            layout_preview_display_conduit.Enabled = false; //TODO _ ONLY FOR TESTS

            //TEST?
            UpdateLocalVariables();
            

        }

        public object Clone()
        {
            return this.MemberwiseClone();
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
            this.paper_size_name = string.Format("{0}mm_x_{1}mm_User_Defined_Paper", this.paper_width, this.paper_height);
            UpdateLocalVariables();
        }

        public void SetPaperOrientation(bool is_landscape)
        {
            UpdatePageOrientation();
            if (this.is_landscape != is_landscape)
            {
                double temp_w = this.paper_height;
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
            this.layout_scale_origin = (Point3d)(this.layout_scale_origin + origin - this.layout_origin);
            this.layout_origin = origin;
            UpdateLocalVariables();
        }

        public void SetLayoutOrigin(double x, double y)
        {
            SetLayoutOrigin(new Point3d(x, y, 0));
        }

        public void SetLayoutScaleOrigin(Point3d origin)
        {
            this.layout_scale_origin = origin;
            UpdateLocalVariables();
        }

        public void SetLayoutScaleOrigin(double x, double y)
        {
            SetLayoutScaleOrigin(new Point3d(x, y, 0));
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
            this.layout_x = Vector3d.ZAxis;
            UpdateLocalVariables();
        }

        public void SetLayoutPlane(Vector3d x_axis)
        {
            this.layout_x = x_axis;
            UpdateLocalVariables();
        }

        public void SetLayoutScalePlane(Point3d origin)
        {
            this.layout_scale_plane = new Plane(origin, this.layout_x);
        }

        public void SetLayoutScalePlane(Point3d origin, Vector3d x_axis)
        {
            this.layout_x = x_axis;
            this.layout_scale_plane = new Plane(origin, x_axis);
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
            //this.SetLayoutScaleOrigin(rectangle.Center);
            this.SetLayoutPlane(this.layout_origin, Vector3d.ZAxis);
            UpdateLocalVariables();
        }

        public void SetLayoutBoundingBox(double width, double height)
        {
            this.model_width = width;
            this.model_height = height;
            this.paper_width = this.model_width / this.scale * 1000;
            this.paper_height = this.model_height / this.scale * 1000;
            this.layout_boundingbox = new Rectangle3d(Plane.WorldXY, this.model_width, this.model_height);
            Point3d center = new Point3d(width / 2, height / 2, 0);
            this.layout_origin = new Point3d(0, 0, 0);
            //SetLayoutScaleOrigin(center);
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
            this.layout_plane = new Plane(this.layout_origin, Vector3d.ZAxis);
            this.layout_scale_plane = new Plane(this.layout_scale_origin, Vector3d.ZAxis); // TODO TEST
        }

        private void UpdateModelDimensions()
        {
            this.model_width = this.paper_width * this.scale / 1000;
            this.model_height = this.paper_height * this.scale / 1000;
        }

        private void UpdateLayoutBoundingBox()
        {
            Transform scaling = Transform.Scale(this.layout_scale_plane, model_width / Math.Abs(this.layout_boundingbox.X.Length), model_height / Math.Abs(this.layout_boundingbox.Y.Length), 1);
            
            this.layout_boundingbox.Transform(scaling);
            this.layout_preview_display_conduit.UpdatePreviewGeometry(this.layout_boundingbox);
        }

        private void UpdateUserDictionary()
        {
            extended_user_dictionary.SetLayoutSetting(this);
            extended_user_dictionary.AddContentsFrom(GenerateArchivableDictionary());
        }


        public Vector3d[] GetBoundingBoxRelativeRelativeTransformVectors(int justification)
        {
            Point3d justified_origin = this.layout_boundingbox.Center;
            switch (justification)
            {
                case 1:
                    justified_origin = this.layout_boundingbox.PointAt(4);
                    break;

                case 2:
                    justified_origin = this.layout_boundingbox.PointAt(1);
                    break;

                case 3:
                    justified_origin = this.layout_boundingbox.PointAt(3);
                    break;

                case 4:
                    justified_origin = this.layout_boundingbox.PointAt(2);
                    break;
            }
            Vector3d vector_0 = new Vector3d(this.layout_boundingbox.PointAt(0) - justified_origin);
            Vector3d vector_1 = new Vector3d(this.layout_boundingbox.PointAt(1) - justified_origin);
            Vector3d vector_2 = new Vector3d(this.layout_boundingbox.PointAt(2) - justified_origin);
            Vector3d vector_3 = new Vector3d(this.layout_boundingbox.PointAt(3) - justified_origin);
            return new Vector3d[4] { vector_0, vector_1, vector_2, vector_3 };
        }
        public void LayoutCreationPreview(bool layout_creation_preview)
        {
            // TODO - ADD ANNOTATIONS AND TEXTS <- CONDUIT CLASS
            this.layout_preview_display_conduit.Enabled = layout_creation_preview;
        }

        public void AddLayout(RhinoDoc doc)
        {
            // TODO
            UpdateLocalVariables();
            
            RhinoPageView[] page_views = doc.Views.GetPageViews();
            List<Guid> layout_guids = new List<Guid>();





            doc.PageUnitSystem = UnitSystem.Millimeters;

            RhinoPageView layout = doc.Views.AddPageView(this.layout_name, paper_width, paper_height);
            Point2d top_left = new Point2d(0, paper_height);
            Point2d bottom_right = new Point2d(paper_width, 0);
            Rhino.DocObjects.DetailViewObject detail = layout.AddDetailView(drawing_name, top_left, bottom_right, DefinedViewportProjection.Top);

            layout.SetActiveDetail(detail.Id);
            Point3d center = new Point3d(0.5 * model_width, 0.5 * model_height, 0);
            detail.Viewport.SetCameraLocation(center, true);
            detail.Viewport.SetCameraTarget(center, true);
            detail.CommitViewportChanges();
            detail.DetailGeometry.IsProjectionLocked = true;
            detail.DetailGeometry.SetScale(scale / 1000, doc.ModelUnitSystem, 1, doc.PageUnitSystem);

            detail.Attributes.UserDictionary.AddContentsFrom(user_dictionary);
            detail.CommitChanges();


            layout.SetPageAsActive();
            doc.Views.ActiveView = layout;
            doc.Views.Redraw();


        }

        public ArchivableDictionary GenerateArchivableDictionary()
        {
            // TODO - EXPERIMENTAL METHOD
            user_dictionary.Set("paper_size_name", paper_size_name);
            user_dictionary.Set("paper_width", paper_width);
            user_dictionary.Set("paper_height", paper_height);
            user_dictionary.Set("is_landscape", is_landscape);

            user_dictionary.Set("scale", scale);
            user_dictionary.Set("drawing_elements_scaling", drawing_elements_scaling);

            user_dictionary.Set("layout_guid", layout_guid);
            user_dictionary.Set("layout_id", layout_id);
            user_dictionary.Set("layout_name", layout_name);
            user_dictionary.Set("layout_origin", layout_origin);
            user_dictionary.Set("layout_scale_origin", layout_scale_origin);
            user_dictionary.Set("layout_plane", layout_plane);
            user_dictionary.Set("layout_scale_plane", layout_scale_plane);
            user_dictionary.Set("model_width", model_width);
            user_dictionary.Set("model_height", model_height);
            user_dictionary.Set("layout_boundingbox", new RectangleF((float)layout_boundingbox.PointAt(0).X, (float)layout_boundingbox.PointAt(0).Y, (float)model_width, (float)model_height));

            user_dictionary.Set("drawing_name", drawing_name);
            user_dictionary.Set("drawing_alt_name", drawing_alt_name);
            user_dictionary.Set("printing_edge", printing_edge);
            user_dictionary.Set("drawing_edge", drawing_edge);

            return extended_user_dictionary;
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

    /*
    public class GenericArchivableDictionary<T> : ArchivableDictionary where T: Getba
    {
        private T t;

        public void SetDialogOption(T dialog_option)
        {
            this.t = dialog_option;
        }

        public T GetDialogOption()
        {
            return this.t;
        }
    }
    */

    public class LayoutOptionDialog
    {
        //private GetOption option = new Rhino.Input.Custom.GetOption();
        
        //private List<string> option_names;
        //private List<string> option_defaults;
        //private List<int> option_indices = new List<int>();

        private ArchivableDictionary result = new ArchivableDictionary();

        public LayoutOptionDialog(GetOption option, string msg, List<string> option_names, List<string> option_defaults)
        {
            AddOptions(option, msg, option_names, option_defaults);
            Get(option);
        }

        public LayoutOptionDialog(GetObject option, string msg, List<string> option_names, List<string> option_defaults)
        {
            AddOptions(option, msg, option_names, option_defaults);
            Get(option);
        }

        public LayoutOptionDialog(GetPoint option, string msg, List<string> option_names, List<string> option_defaults)
        {
            AddOptions(option, msg, option_names, option_defaults);
            Get(option);
        }

        public LayoutOptionDialog(GetOption option, string msg, List<string> option_names)
        {
            AddOptions(option, msg, option_names);
            Get(option);
        }

        public LayoutOptionDialog(GetObject option, string msg, List<string> option_names)
        {
            AddOptions(option, msg, option_names);
            Get(option);
        }

        public LayoutOptionDialog(GetPoint option, string msg, List<string> option_names)
        {
            AddOptions(option, msg, option_names);
            Get(option);
        }

        public LayoutOptionDialog(GetString option, string msg)
        {
            option.SetCommandPrompt(msg);
            Get(option);
        }

        public LayoutOptionDialog(GetPoint option, string msg)
        {
            option.SetCommandPrompt(msg);
            Get(option);
        }

        public LayoutOptionDialog(GetObject option, string msg)
        {
            option.SetCommandPrompt(msg);
            Get(option);
        }

        public LayoutOptionDialog(GetNumber option, string msg)
        {
            option.SetCommandPrompt(msg);
            Get(option);
        }

        private void AddOptions(GetBaseClass option, string msg, List<string> option_names, List<string> option_defaults)
        {
            option.SetCommandPrompt(msg);
            for (int i = 0; i < option_names.Count; i++)
            {
                option.AddOption(option_names[i], option_defaults[i]);
            }
        }

        private void AddOptions(GetBaseClass option, string msg, List<string> option_names)
        {
            option.SetCommandPrompt(msg);
            for (int i = 0; i < option_names.Count; i++)
            {
                option.AddOption(option_names[i]);
            }
        }

        private void Get(GetOption option)
        {
            while (true)
            {
                option.Get();

                if (option.Result() == GetResult.Option)
                {
                    this.result.Set("choice_index" , option.Option().Index);
                    break;
                }
                if (option.Result() == GetResult.Cancel)
                {
                    this.result.Set("choice_index", -1);
                    break;
                }
                if ((option.Result() == GetResult.Nothing) | (option.Result() == GetResult.NoResult))
                {
                    this.result.Set("choice_index", 0);
                    break;
                }
            }
        }

        private void Get(GetPoint option)
        {
            while (true)
            {
                option.Get();
                if (option.Result() == GetResult.Option)
                {
                    this.result.Set("choice_index", 0);
                    break;
                }
                if (option.Result() == GetResult.Cancel)
                {
                    this.result.Set("choice_index", -1);
                    break;
                }
                if (option.Result() == GetResult.Point)
                {
                    this.result.Set("choice_index", 1);
                    this.result.Set("result", option.Point());
                    break;
                }
            }
        }

        private void Get(GetString option)
        {
            while (true)
            {
                option.Get();
                if (option.Result() == GetResult.Option)
                {
                    this.result.Set("choice_index", 0);
                    break;
                }
                if (option.Result() == GetResult.Cancel)
                {
                    this.result.Set("choice_index", -1);
                    break;
                }
                if (option.Result() == GetResult.String)
                {
                    this.result.Set("choice_index", 1);
                    this.result.Set("result", option.StringResult());
                    break;
                }
            }
        }

        private void Get(GetNumber option)
        {
            while (true)
            {
                option.Get();
                if (option.Result() == GetResult.Option)
                {
                    this.result.Set("choice_index", 0);
                    break;
                }
                if (option.Result() == GetResult.Cancel)
                {
                    this.result.Set("choice_index", -1);
                    break;
                }
                if (option.Result() == GetResult.Number)
                {
                    this.result.Set("choice_index", 1);
                    this.result.Set("result", option.Number());
                    break;
                }
            }
        }

        private void Get(GetObject option)
        {
            while (true)
            {
                option.GetMultiple(1, 0);
                if (option.Result() == GetResult.Option)
                {
                    this.result.Set("choice_index", 0);
                }
                if (option.Result() == GetResult.Cancel)
                {
                    this.result.Set("choice_index", -1);
                    break;
                }
                if (option.Result() == GetResult.Object)
                {
                    this.result.Set("choice_index", 1);
                    this.result.Set("result", option.Objects());
                    break;
                }
            }
        }

        public ArchivableDictionary DialogResult()
        {
            return this.result;
        }
    }

    public class ObjectListBoundingBox
    {
        BoundingBox box;
        public ObjectListBoundingBox(Rhino.DocObjects.ObjRef[] objrefs, bool accurate)
        {   
            
            
            GeometryBase geometry_base = objrefs[0].Geometry();
            box = geometry_base.GetBoundingBox(accurate);

            for (int i = 1; i < objrefs.Length; i++)
            {
                geometry_base = objrefs[i].Geometry();
                box.Union(geometry_base.GetBoundingBox(accurate));
            }
        }

        public BoundingBox Box()
        {
            return box;
        }
    }


    public class DisplayConduitLayoutPreview : DisplayConduit
    {
        public Line[] lines;
        public DisplayConduitLayoutPreview(Rectangle3d rectangle)
        {
            this.lines = rectangle.ToPolyline().GetSegments();
        }

        public DisplayConduitLayoutPreview()
        {
            this.lines = new Line[] { };
        }

        public DisplayConduitLayoutPreview(Line line)
        {
            this.lines = new Line[1] { line };
        }

        public DisplayConduitLayoutPreview(Line[] lines)
        {
            this.lines = lines;
        }

        public void UpdatePreviewGeometry(Rectangle3d rectangle)
        {
            this.lines = rectangle.ToPolyline().GetSegments();
        }
        
        protected override void PostDrawObjects(DrawEventArgs e)
        {   
            if (this.lines.Length > 0)
            {
                for (int i = 0; i < this.lines.Length; i ++)
                {
                    LineCurve line_curve = new LineCurve(this.lines[i]);
                    e.Display.DrawLine(line_curve.PointAtStart, line_curve.PointAtEnd, Color.White, 2);

                }
            }
        }
    }
    

    public class PluginConfig
    {
        private StringTable string_table;

        public PluginConfig(RhinoDoc doc)
        {
            string_table = doc.Strings;
        }

        public bool DisplayLayoutCreationPreviews()
        {
            if (string_table.GetValue("RPH_display_layout_creation_previews") == "true")
            {
                return true;
            }
            string_table.SetString("RPH_display_layout_creation_previews", "false");
            return false;
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