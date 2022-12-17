using Apsitvarkom.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

#nullable disable

namespace Apsitvarkom.DataAccess.Migrations;

/// <inheritdoc />
public partial class DatabaseTriggers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "CREATE FUNCTION checkIfProgressIsLessThan100() " +
            "RETURNS trigger AS " +
            "$$ " +
            "BEGIN " +
            "IF (SELECT PollutedLocations.Progress from PollutedLocations where PollutedLocations.Id = NEW.Id) = 100 " +
            "THEN RAISE EXCEPTION 'Cannot update polluted location because progress is allready set to 100'; " +
            "END IF; " +
            "RETURN NEW; " +
            "END; $$ " +
            "LANGUAGE plpgsql; " +
            "CREATE TRIGGER CHECK_PROGRESS_BEFORE_POLLUTED_LOCATION_UPDATE " +
            "BEFORE UPDATE ON PollutedLocations FOR EACH ROW EXECUTE PROCEDURE checkIfProgressIsLessThan100();"
        );

        migrationBuilder.Sql(
           "CREATE FUNCTION checkIfNotDecreasingProgress() " +
           "RETURNS trigger AS " +
           "$$ " +
           "BEGIN " +
           "IF (SELECT PollutedLocations.Progress from PollutedLocations where PollutedLocations.Id = NEW.Id) > NEW.Progress " +
           "THEN RAISE EXCEPTION 'Cannot decrease progress'; " +
           "END IF; " +
           "RETURN NEW; " +
           "END;$$ " +
           "LANGUAGE plpgsql; " +
           "CREATE TRIGGER CHECK_IF_PROGRESS_NOT_DECREASED " +
           "BEFORE UPDATE OF PROGRESS ON PollutedLocations FOR EACH ROW EXECUTE PROCEDURE checkIfNotDecreasingProgress();"
       );

        migrationBuilder.Sql(
           "CREATE FUNCTION doNotUpdateIfEventIsFinalized() " +
           "RETURNS trigger AS " +
           "$$ " +
           "BEGIN " +
           "IF (SELECT CleaningEvents.isFinalized from CleaningEvents where CleaningEvents.Id = NEW.Id) IS NOT NULL " +
           "THEN RAISE EXCEPTION 'Cannot update finalized events'; " +
           "END IF; " +
           "RETURN NEW; " +
           "END; $$ " +
           "LANGUAGE plpgsql; " +
           "CREATE TRIGGER CHECK_IF_EVENT_NOT_FINALIZED " +
           "BEFORE UPDATE ON CleaningEvents FOR EACH ROW EXECUTE PROCEDURE doNotUpdateIfEventIsFinalized(); "
       );

        migrationBuilder.Sql(
           "CREATE FUNCTION doNotUpdateEventIfNotStarted() " +
           "RETURNS trigger AS " +
           "$$ " +
           "BEGIN " +
           "IF (SELECT CleaningEvents.StartTime from CleaningEvents where CleaningEvents.Id = NEW.Id) < select now() at time zone ('utc'); " +
           "THEN RAISE EXCEPTION 'Cannot update events that did not start'; " +
           "END IF; " +
           "RETURN NEW; " +
           "END; $$ " +
           "LANGUAGE plpgsql; " +
           "CREATE TRIGGER CHECK_IF_EVENT_STARTED " +
           "BEFORE UPDATE ON CleaningEvents FOR EACH ROW EXECUTE PROCEDURE doNotUpdateEventIfNotStarted();"
       );

        migrationBuilder.Sql(
           "CREATE FUNCTION checkIfLocationProgressIsNot100() " +
           "RETURNS trigger AS " +
           "$$ " +
           "BEGIN " +
           "IF (SELECT PollutedLocations.Progress from PollutedLocations where PollutedLocations.Id = NEW.PollutedLocationId) = 100; " +
           "THEN RAISE EXCEPTION 'Cannot create events for locations that are already cleaned'; " +
           "END IF; " +
           "RETURN NEW; " +
           "END; $$ " +
           "LANGUAGE plpgsql; " +
           "CREATE TRIGGER CHECK_IF_LOCATION_IS_NOT_CLEANED " +
           "BEFORE INSERT ON CleaningEvents FOR EACH ROW EXECUTE PROCEDURE checkIfLocationProgressIsNot100(); "
       );

        migrationBuilder.Sql(
            "CREATE FUNCTION checkIfStartDateIsNotInPast() " +
            "RETURNS trigger AS " +
            "$$ " +
            "BEGIN " +
            "IF (NEW.StartTime) < select now() at time zone ('utc'); " +
            "THEN RAISE EXCEPTION 'Cannot create events in the past'; " +
            "END IF; " +
            "RETURN NEW; " +
            "END; $$ " +
            "LANGUAGE plpgsql; " +
            "CREATE TRIGGER CHECK_IF_START_DATE_IS_VALID " +
            "BEFORE INSERT OR UPDATE ON CleaningEvents FOR EACH ROW EXECUTE PROCEDURE checkIfStartDateIsNotInPast(); "
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION checkIfProgressIsLessThan100 CASCADE;");
        migrationBuilder.Sql("DROP FUNCTION checkIfNotDecreasingProgress CASCADE;");
        migrationBuilder.Sql("DROP FUNCTION doNotUpdateIfEventIsFinalized CASCADE;");
        migrationBuilder.Sql("DROP FUNCTION doNotUpdateEventIfNotStarted CASCADE;");
        migrationBuilder.Sql("DROP FUNCTION checkIfLocationProgressIsNot100 CASCADE;");
        migrationBuilder.Sql("DROP FUNCTION checkIfStartDateIsNotInPast CASCADE;");
    }

}
