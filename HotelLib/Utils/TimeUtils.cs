namespace HotelLib.Utils;

public static class TimeUtils
{
    public static string Format(double time)
    {
        if (time < 0)
            time = Math.Abs(time);

        var months = (int)time / 2592000;
        var days = (int)time / 86400 % 30;
        var hours = (int)time / 3600 % 24;
        var minutes = (int)time / 60 % 60;
        var seconds = (int)time % 60;

        var timeString = "";

        if (months > 0)
            timeString += $"{months}mon ";
        else if (days > 0)
            timeString += $"{days}d ";
        else if (hours > 0)
            timeString += $"{hours}h ";
        else if (minutes > 0)
            timeString += $"{minutes}m ";
        else if (seconds > 0)
            timeString += $"{seconds}s ";

        return timeString.TrimEnd();
    }
}
