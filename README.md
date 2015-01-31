# Nina WebAPI

## Setup
In Global.asax, set:

    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.Add(new Route("{resource}.axd/{*pathInfo}", new StopRoutingHandler()));
        routes.Add(new Nina.MountingPoint("~/api", null));
    }

    protected void Application_Start()
    {
        RegisterRoutes(RouteTable.Routes);
    }

## Modules

    pubic class Home : Nina.Module
    {
        [Nina.Get("/now/:days")]
        public DateTime GetNow(int days)
        {
            return DateTime.Now;
        }
    }

## Model Binding

Nina use json based data model.