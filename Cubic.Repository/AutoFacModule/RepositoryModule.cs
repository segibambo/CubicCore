using Autofac;
using Cubic.Data;
using Cubic.Repository.CoreRepositories;

namespace Cubic.Repository.AutoFacModule
{
    public class RepositoryModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {


            builder.RegisterType<APPContext>().InstancePerLifetimeScope();
           

            builder.RegisterGeneric(typeof(RepositoryCommand<,>))
                .As(typeof(IRepositoryCommand<,>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(RepositoryQuery<,>))
               .As(typeof(IRepositoryQuery<,>))
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IAutoDependencyRegister).Assembly)
                .AssignableTo<IAutoDependencyRegister>()
                .As<IAutoDependencyRegister>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            base.Load(builder);
        }
    }
}