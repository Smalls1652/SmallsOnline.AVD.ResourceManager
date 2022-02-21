namespace SmallsOnline.AVD.ResourceManager.Models.Json;

/// <summary>
/// A custom <see cref="System.Text.Json" /> converter for the <see cref="DateTimeOffset" /> type.
/// </summary>
public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    /// <summary>
    /// A custom defintion of the Eastern Standard Time timezone to account for systems that don't have timezones.
    /// </summary>
    private readonly TimeZoneInfo _dateTimeTimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(
            id: "Eastern Standard Time",
            baseUtcOffset: new(-5, 0, 0),
            displayName: "Eastern Time",
            standardDisplayName: "Eastern Standard Time",
            daylightDisplayName: "Eastern Daylight Time",
            adjustmentRules: new[]
            {
                TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    dateStart: new(1, 1, 1, 0, 0, 0),
                    dateEnd: new(2006, 12, 31, 0, 0, 0),
                    daylightDelta: new(1, 0, 0),
                    daylightTransitionStart: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                        timeOfDay: new(1, 1, 1, 2, 0, 0),
                        month: 4,
                        week: 1,
                        dayOfWeek: DayOfWeek.Sunday
                    ),
                    daylightTransitionEnd: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                        timeOfDay: new(1, 1, 1, 2, 0, 0),
                        month: 10,
                        week: 5,
                        dayOfWeek: DayOfWeek.Sunday
                    ),
                    baseUtcOffsetDelta: new(0, 0, 0)
                ),
                TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    dateStart: new(2007, 1, 1, 0, 0, 0),
                    dateEnd: new(9999, 12, 31, 0, 0, 0),
                    daylightDelta: new(1, 0, 0),
                    daylightTransitionStart: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                        timeOfDay: new(1, 1, 1, 2, 0, 0),
                        month: 3,
                        week: 2,
                        dayOfWeek: DayOfWeek.Sunday
                    ),
                    daylightTransitionEnd: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                        timeOfDay: new(1, 1, 1, 2, 0, 0),
                        month: 11,
                        week: 1,
                        dayOfWeek: DayOfWeek.Sunday
                    ),
                    baseUtcOffsetDelta: new(0, 0, 0)
                )
            }
        );
        
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        DateTime inputTime = DateTime.Parse($"{reader.GetString()} 17:00");
        return new(inputTime, _dateTimeTimeZoneInfo.GetUtcOffset(inputTime));
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}