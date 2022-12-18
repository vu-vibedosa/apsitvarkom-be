using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apsitvarkom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "create function Update_PollutedLocationAlreadyCleanedUp() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select \"PollutedLocations\".\"Progress\" = 100 from \"PollutedLocations\" " +
                   "where \"PollutedLocations\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not update the polluted location, as its cleaning up process is already complete (Progress = 100).'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_PollutedLocationAlreadyCleanedUp " +
                "before update on \"PollutedLocations\" " +
                "for each row " +
                "execute procedure Update_PollutedLocationAlreadyCleanedUp();"
            );

            migrationBuilder.Sql(
                "create function Update_PollutedLocationProgressDecreased() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select \"PollutedLocations\".\"Progress\" > new.\"Progress\" from \"PollutedLocations\" " +
                   "where \"PollutedLocations\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not update the polluted location, as its progress cannot decrease.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_PollutedLocationProgressDecreased " +
                "before update on \"PollutedLocations\" " +
                "for each row " +
                "execute procedure Update_PollutedLocationProgressDecreased();"
            );

            migrationBuilder.Sql(
                "create function Update_CleaningEventIsFinalized() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select \"CleaningEvents\".\"IsFinalized\" = true from \"CleaningEvents\" " +
                   "where \"CleaningEvents\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not update the cleaning event, as it is already finalized.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_CleaningEventIsFinalized " +
                "before update on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Update_CleaningEventIsFinalized();"
            );

            migrationBuilder.Sql(
                "create function Update_CleaningEventNotFinished_TriedToBeFinalized() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "\"CleaningEvents\".\"StartTime\" > NOW() from \"CleaningEvents\" " +
                   "where \"CleaningEvents\".\"Id\" = new.\"Id\" and new.\"IsFinalized\" = true " +
                   ") " +
                   "then " +
                   "raise exception 'Could not finalize the cleaning event, as it has not yet happened.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_CleaningEventNotFinished_TriedToBeFinalized " +
                "before update on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Update_CleaningEventNotFinished_TriedToBeFinalized();"
            );

            migrationBuilder.Sql(
                "create function Insert_CannotInsertCleaningEventsForCleanedUpPollutedLocations() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "\"PollutedLocations\".\"Progress\" = 100 from \"PollutedLocations\" " +
                   "where \"PollutedLocations\".\"Id\" = new.\"PollutedLocationId\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not create cleaning event for already cleaned up polluted location.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Insert_CannotInsertCleaningEventsForCleanedUpPollutedLocations " +
                "before insert on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Insert_CannotInsertCleaningEventsForCleanedUpPollutedLocations();"
            );

            migrationBuilder.Sql(
                "create function Insert_CannotInsertCleaningEventsIfPollutedLocationHasActiveCleaningEventAlready() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "count(*) > 0 from \"CleaningEvents\" " +
                   "where \"CleaningEvents\".\"PollutedLocationId\" = new.\"PollutedLocationId\" and \"CleaningEvents\".\"IsFinalized\" = false " +
                   ") " +
                   "then " +
                   "raise exception 'Could not create cleaning event as the polluted location has an active cleaning event already.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Insert_CannotInsertCleaningEventsIfPollutedLocationHasActiveCleaningEventAlready " +
                "before insert on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Insert_CannotInsertCleaningEventsIfPollutedLocationHasActiveCleaningEventAlready();"
            );

            migrationBuilder.Sql(
                "create function Alter_CannotSetCleaningEventStartTimeInThePast() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "new.\"StartTime\" < now() " +
                   ") " +
                   "then " +
                   "raise exception 'Could not set cleaning event start time in the past.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Insert_CannotSetCleaningEventStartTimeInThePast " +
                "before insert on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Alter_CannotSetCleaningEventStartTimeInThePast(); " +

                "create trigger Update_CannotSetCleaningEventStartTimeInThePast " +
                "before update on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Alter_CannotSetCleaningEventStartTimeInThePast();"
            );

            migrationBuilder.Sql(
                "create function Update_CleaningEventPollutedLocationIdCannotBeChanged() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "\"CleaningEvents\".\"PollutedLocationId\" <> new.\"PollutedLocationId\" from \"CleaningEvents\" " +
                   "where \"CleaningEvents\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not change PollutedLocationId. This property is read-only. If you want to change it, re-create the Cleaning Event.'; " +
                   "end if; " +
                   "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_CleaningEventPollutedLocationIdCannotBeChanged " +
                "before update on \"CleaningEvents\" " +
                "for each row " +
                "execute procedure Update_CleaningEventPollutedLocationIdCannotBeChanged();"
            );

            migrationBuilder.Sql(
                "create function Update_PollutedLocationSpottedCannotBeUpdated() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "\"PollutedLocations\".\"Spotted\" <> new.\"Spotted\" from \"PollutedLocations\" " +
                   "where \"PollutedLocations\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not change Spotted. This property is read-only. If you want to change it, re-create the Polluted Location.';  " +
                   "end if; " +
                "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_PollutedLocationSpottedCannotBeUpdated " +
                "before update on \"PollutedLocations\" " +
                "for each row " +
                "execute procedure Update_PollutedLocationSpottedCannotBeUpdated();"
            );            
            
            migrationBuilder.Sql(
                "create function Update_PollutedLocationLocationTitleCannotBeUpdated() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "\"PollutedLocations\".\"Location_Title_English\" <> new.\"Location_Title_English\" or" +
                   "\"PollutedLocations\".\"Location_Title_Lithuanian\" <> new.\"Location_Title_Lithuanian\"" +
                   " from \"PollutedLocations\" " +
                   "where \"PollutedLocations\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not change Location_Title. This property is read-only. If you want to change it, re-create the Polluted Location.';  " +
                   "end if; " +
                "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_PollutedLocationLocationTitleCannotBeUpdated " +
                "before update on \"PollutedLocations\" " +
                "for each row " +
                "execute procedure Update_PollutedLocationLocationTitleCannotBeUpdated();"
            );           
            
            migrationBuilder.Sql(
                "create function Update_PollutedLocationLocationCoordinatesCannotBeUpdated() " +
                "returns trigger as " +
                "$$ begin " +
                   "if ( " +
                   "select " +
                   "\"PollutedLocations\".\"Location_Coordinates_Latitude\" <> new.\"Location_Coordinates_Latitude\" or" +
                   "\"PollutedLocations\".\"Location_Coordinates_Longitude\" <> new.\"Location_Coordinates_Longitude\"" +
                   " from \"PollutedLocations\" " +
                   "where \"PollutedLocations\".\"Id\" = new.\"Id\" " +
                   ") " +
                   "then " +
                   "raise exception 'Could not change Location_Coordinates. This property is read-only. If you want to change it, re-create the Polluted Location.';  " +
                   "end if; " +
                "return new; " +
                "end; $$ " +
                "language plpgsql; " +

                "create trigger Update_PollutedLocationLocationCoordinatesCannotBeUpdated " +
                "before update on \"PollutedLocations\" " +
                "for each row " +
                "execute procedure Update_PollutedLocationLocationCoordinatesCannotBeUpdated();"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop function Update_PollutedLocationAlreadyCleanedUp cascade;");
            migrationBuilder.Sql("drop function Update_PollutedLocationProgressDecreased cascade;");
            migrationBuilder.Sql("drop function Update_CleaningEventIsFinalized cascade;");
            migrationBuilder.Sql("drop function Update_CleaningEventNotFinished_TriedToBeFinalized cascade;");
            migrationBuilder.Sql("drop function Insert_CannotInsertCleaningEventsForCleanedUpPollutedLocations cascade;");
            migrationBuilder.Sql("drop function Insert_CannotInsertCleaningEventsIfPollutedLocationHasActiveCleaningEventAlready cascade;");
            migrationBuilder.Sql("drop function Alter_CannotSetCleaningEventStartTimeInThePast cascade;");
            migrationBuilder.Sql("drop function Update_CleaningEventPollutedLocationIdCannotBeChanged cascade;");
            migrationBuilder.Sql("drop function Update_PollutedLocationSpottedCannotBeUpdated cascade;");
            migrationBuilder.Sql("drop function Update_PollutedLocationLocationTitleCannotBeUpdated cascade;");
            migrationBuilder.Sql("drop function Update_PollutedLocationLocationCoordinatesCannotBeUpdated cascade;");
        }
    }
}
