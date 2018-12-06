using FDCommonTools.SqlTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using System.Linq;

namespace TimeTracker.DAL.Repositories
{
    public class VolunteerOpportunityRepository : IVolunteerOpportunity
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        private const string _baseQuery = @"
                            SELECT 
                                volopps.[VolunteerOpportunityId]
                                ,volopps.[OpportunityName]
                                ,volopps.[IsOffsiteEvent]
                                ,volopps.[IsVisible]
                                ,volopps.[CreditValue]
                                ,volopps.[OpportunityDescription]
                                ,volopps.[OpportunityStartDateTime]
                                ,volopps.[OpportunityEndDateTime]
                            FROM [VolunteerOpportunities] volopps";

        public Guid CreateVolunteerOpportunity(VolunteerOpportunity newOpportunity)
        {
            string sql = @"
                            INSERT INTO [dbo].[VolunteerOpportunities]
                                (
		                            [OpportunityName]
		                            ,[IsOffsiteEvent]
                                    ,[IsVisible]
                                    ,[CreditValue]
                                    ,[OpportunityDescription]
                                    ,[OpportunityStartDateTime]
                                    ,[OpportunityEndDateTime]
                                )
                            OUTPUT inserted.VolunteerOpportunityId
                            VALUES
                                (
		                            @OpportunityName
		                            ,@IsOffsiteEvent
                                    ,@IsVisible
                                    ,@CreditValue
                                    ,@Description
                                    ,@StartDateTime
                                    ,@EndDateTime
                                )";

            var parameters = new[]
            {
                new SqlParameter("@OpportunityName", newOpportunity.Name),
                new SqlParameter("@IsOffsiteEvent", newOpportunity.IsOffsite.IsChecked),
                new SqlParameter("@IsVisible", newOpportunity.IsVisibleToVolunteer.IsChecked),
                new SqlParameter("@CreditValue", newOpportunity.CreditValue),
                new SqlParameter("@Description", newOpportunity.Description ?? ""),
                new SqlParameter("@StartDateTime", newOpportunity.StartDateTime ?? (object)DBNull.Value),
                new SqlParameter("@EndDateTime", newOpportunity.EndDateTime ?? (object)DBNull.Value)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                newOpportunity.Id = result;

            return newOpportunity.Id;
        }

        public List<VolunteerOpportunity> GetVisibleOpportunities()
        {
            List<VolunteerOpportunity> opps = new List<VolunteerOpportunity>();


            string sql = @"
                            SELECT 
                                volopps.[VolunteerOpportunityId]
                                ,volopps.[OpportunityName]
                                ,volopps.[IsOffsiteEvent]
                                ,volopps.[IsVisible]
                                ,volopps.[CreditValue]
                                ,volopps.[OpportunityDescription]
                                ,volopps.[OpportunityStartDateTime]
                                ,volopps.[OpportunityEndDateTime]
                            FROM [VolunteerOpportunities] volopps
                            WHERE volopps.IsVisible = '1'
	                            AND (OpportunityStartDateTime >= GETDATE() AND OpportunityEndDateTime >= GETDATE()
		                             OR OpportunityStartDateTime IS NULL AND OpportunityEndDateTime IS NULL
                                     OR GETDATE() BETWEEN OpportunityStartDateTime AND OpportunityEndDateTime)";

            var result = _helper.ExecSqlPullDataTable(sql);

            if (Tools.DataTableHasRows(result))
                opps = (from DataRow row in result.Rows select Maps.VolunteerOpportunityMaps.MapVolunteerOpportunity(row)).ToList();

            return opps;
        }

        public List<VolunteerOpportunity> GetVolunteerOpportunities()
        {
            List<VolunteerOpportunity> opportunities = new List<VolunteerOpportunity>();

            var result = _helper.ExecSqlPullDataTable(_baseQuery);

            if (result != null)
                opportunities = (from DataRow row in result.Rows select Maps.VolunteerOpportunityMaps.MapVolunteerOpportunity(row)).ToList();

            return opportunities;
        }

        public void UpdateVolunteerOpportunity(VolunteerOpportunity opportunity)
        {
            string sql = @"
                            UPDATE [dbo].[VolunteerOpportunities]
                            SET [OpportunityName] = @OpportunityName
                                ,[CreditValue] = @CreditValue
                                ,[IsOffsiteEvent] = @IsOffsiteEvent
                                ,[IsVisible] = @IsVisible
                                ,[OpportunityDescription] = @Description
                                ,[OpportunityStartDateTime] = @StartDateTime
                                ,[OpportunityEndDateTime] = @EndDateTime
                            WHERE VolunteerOpportunityId = @OpportunityId";

            var parameters = new[]
            {
                new SqlParameter("@OpportunityName", opportunity.Name),
                new SqlParameter("@CreditValue", opportunity.CreditValue),
                new SqlParameter("@IsOffsiteEvent", opportunity.IsOffsite.IsChecked),
                new SqlParameter("@IsVisible", opportunity.IsVisibleToVolunteer.IsChecked),
                new SqlParameter("@Description", opportunity.Description),
                new SqlParameter("@StartDateTime", opportunity.StartDateTime ?? (object)DBNull.Value),
                new SqlParameter("@EndDateTime", opportunity.EndDateTime ?? (object)DBNull.Value),
                new SqlParameter("@OpportunityId", opportunity.Id)
            };

            var result = _helper.ExecScalarSqlPullObject(sql, parameters);
        }

        public VolunteerOpportunity GetVolunteerOpportunityById(Guid volOppId)
        {
            VolunteerOpportunity volOpp = new VolunteerOpportunity();

            string sql = _baseQuery + @" WHERE VolunteerOpportunityId = @VolOppId";

            var parameters = new[]
            {
                new SqlParameter("@VolOppId", volOppId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                volOpp = (from DataRow row in result.Rows select Maps.VolunteerOpportunityMaps.MapVolunteerOpportunity(row)).FirstOrDefault();

            return volOpp;
        }
    }
}
