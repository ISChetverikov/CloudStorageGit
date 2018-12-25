using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class ClientSettings
    {
        public string InputFolder { get; set; }

        public string TempFolder { get; set; }

        public string LogFolder { get; set; }

        public string ServerAddress { get; set; }

        public bool IsScheduled { get; set; }

        public TimeSpan SchedulePeriod { get;set;}

        public override bool Equals(Object other)
        {
            if ((other == null) || !this.GetType().Equals(other.GetType()))
            {
                return false;
            }
            else
            {
                bool result = true;

                var otherSettings = (ClientSettings)other;
                result = result && (InputFolder == otherSettings.InputFolder);
                result = result && (TempFolder == otherSettings.TempFolder);
                result = result && (LogFolder == otherSettings.LogFolder);
                result = result && (ServerAddress == otherSettings.ServerAddress);
                result = result && (IsScheduled == otherSettings.IsScheduled);
                result = result && (SchedulePeriod == otherSettings.SchedulePeriod);

                return result;
            }
        }

        public static bool operator ==(ClientSettings left, ClientSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ClientSettings left, ClientSettings right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            var hashCode = -122412166;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InputFolder);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TempFolder);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LogFolder);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ServerAddress);
            hashCode = hashCode * -1521134295 + IsScheduled.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(SchedulePeriod);
            return hashCode;
        }
    }
}
