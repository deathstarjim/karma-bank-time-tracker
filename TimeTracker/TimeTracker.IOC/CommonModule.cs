using Ninject.Modules;
using TimeTracker.Core.Contracts;
using TimeTracker.DAL.Repositories;

namespace TimeTracker.IOC
{
    public class CommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IVolunteer>().To<VolunteerRepository>();
            Bind<IVolTimePunch>().To<VolTimePunchRepository>();
            Bind<IVolunteerOpportunity>().To<VolunteerOpportunityRepository>();
            Bind<ILogin>().To<LoginRepository>();
            Bind<ISystemRole>().To<SystemRoleRepository>();
            Bind<ISecurity>().To<SecurityRepository>();
            Bind<IAdministrator>().To<AdministratorsRepository>();
            Bind<IVolTransaction>().To<VolunteerTransactionRepository>();
            Bind<IAdminTimePunch>().To<AdminTimePunchRepository>();
            Bind<IAdminTransaction>().To<AdministratorTransactionRepository>();
            Bind<IReports>().To<ReportsRepository>();
            Bind<ITransactionType>().To<TransactionTypesRepository>();
        }
    }
}
