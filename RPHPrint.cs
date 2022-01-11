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
            String[] path_sep = new String[1] { ".3dm" };
            String path = doc.Path.Split(path_sep, StringSplitOptions.None)[1];
            String msg = String.Format("Current Directory: {}", path);
            var pages = doc.Views.GetPageViews();

            
            return Result.Success;
        }
    }
}