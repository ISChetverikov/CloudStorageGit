using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.File;
using System.IO;

namespace Client
{
    public static class FileActionTypesConverter
    {
        public static FileActionType Convert(WatcherChangeTypes type)
        {
            FileActionType result;

            switch (type)
            {
                case WatcherChangeTypes.Created:
                    result = FileActionType.Created;
                    break;
                case WatcherChangeTypes.Deleted:
                    result = FileActionType.Deleted;
                    break;
                case WatcherChangeTypes.Changed:
                    result = FileActionType.Changed;
                    break;
                case WatcherChangeTypes.Renamed:
                    result = FileActionType.Renamed;
                    break;
                case WatcherChangeTypes.All:
                    result = FileActionType.Changed;
                    break;
                default:
                    result = FileActionType.Changed;
                    break;
            }

            return result;
        }
    }
}
