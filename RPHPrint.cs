using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
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
                String msg = String.Format("Current output directory is: {0}, do you wish a change?", path);



                //GetOption get_dir = new GetOption();
                Boolean change_path = false;
                RhinoGet.GetBool(msg, true, "NO", "YES", ref change_path);

            }

            var pages = doc.Views.GetPageViews();

            
            return Result.Success;
        }
    }
}