using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvent;

public class TimerDataProxy : DataProxy
{
    YearDateChangeEventArgs yearDateChangeEventArgs;
    MonthDateChangeEventArgs monthDateChangeEventArgs;
    DayDateChangeEventArgs dayDateChangeEventArgs;
    HourDateChangeEventArgs hourDateChangeEventArgs;
    MinuteDateChangeEventArgs minuteDateChangeEventArgs;

    private int minute = 0;
    private int hour = 0;
    private int day = 1;
    private int month = 1;
    private int year = 1;

    public int Minute
    {
        get => minute;
        set
        {
             minuteDateChangeEventArgs.minute = this.minute = value;
            // EventManager.Dispatch(minuteDateChangeEventArgs);
            // DataChange();
        }
    }
    public int Hour
    {
        get => hour;
        set
        {
            hourDateChangeEventArgs.hour = this.hour = value;
            // EventManager.Dispatch(hourDateChangeEventArgs);
            // DataChange();
        }
    }
    public int Day
    {
        get => day;
        set
        {
            dayDateChangeEventArgs.day = this.day = value;
            // EventManager.Dispatch(dayDateChangeEventArgs);
            // DataChange();
        }
    }
    public int Month
    {
        get => month;
        set
        {
            monthDateChangeEventArgs.month = this.month = value;
            // EventManager.Dispatch(monthDateChangeEventArgs);
            // DataChange();
        }
    }
    public int Year
    {
        get => year;
        set
        {
            yearDateChangeEventArgs.year = this.year = value;
            // EventManager.Dispatch(yearDateChangeEventArgs);
            // DataChange();
        }
    }

    public void Init(string json)
    {
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(json);
        this.year = (int)jsonData["year"];
        this.month = (int)jsonData["month"];
        this.day = (int)jsonData["day"];
        this.hour = (int)jsonData["hour"];
        this.minute = (int)jsonData["minute"];
    }

    public string GetDataJson()
    {
        Dictionary<string, int> keyValuePairs = new()
        {
            { "minute", minute },
            { "hour", hour },
            { "day", day },
            { "month", month },
            { "year", year },
        };

        return LitJson.JsonMapper.ToJson(keyValuePairs);
    }
}
