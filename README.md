# Nina WebAPI

## Setup

1) Edit your web.config

```XML
<?xml version="1.0"?>
<configuration>
  <appSettings>
    <add key="Nina.Path" value="~/api"/>
    <add key="Nina.Cors" value="*"/>
  </appSettings>
  <system.web>
    <compilation targetFramework="4.5" debug="true"/>
    <httpModules>
      <add name="Nina" type="Nina.Startup, Nina"/>
    </httpModules>
  </system.web>
  <system.webServer>
    <validation validateIntegratedModeConfiguration="false"/>
    <modules>
      <add name="Nina" type="Nina.Startup, Nina"/>
    </modules>
  </system.webServer>
  <location path="api" allowOverride="true">
    <system.webServer>
      <handlers>
        <clear/>
        <add name="ExtensionlessUrl-Integrated-4.0" path="*" verb="GET,HEAD,POST,DEBUG,PUT,DELETE" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" responseBufferLimit="0"/>
      </handlers>
    </system.webServer>
  </location>
</configuration>
```

**ATENTION**: Always re-build after change your `web.config`

2) Write your `api` modules (see below)

## Modules

Modules class must inherit from `NinaModule`. With you want, decorate class with `[BaseUrl]` to set base url.

```C#
[BaseUrl("/users")]
pubic class UserApi : NinaModule
{
    [Get("/{id}")]
    public User GetUser(int id)
    {
        // GET /api/users/123
    }

    [Post("/{id}")]
    public int Post(UserView user)
    {
        // PUT /api/users
    }
    
    [Get("/code/{id:(\\[a-z]{8})}")]
    public string GetCode(string id)
    {
        // GET /api/users/code/abcdefgh
    }
    
    [ValidateInput] // Validate request data for html tags
    [Put("/{id}")]
    public int Post(int id, NameValueCollection data, UserView user, JObejct user2, HttpFileCollection files, Stream input)
    {
        // PUT /api/users/123
    }
}
```

## Model Binding

Parameters bind rules:

- If parameter name found on route pattern? Use route value (`/edit/{id}`)
- If parameter name not found on route:
    - Parameter is `String`? Use `Request.Form.ToString()`
    - Parameter is `NameValueCollection`? Use `Request.Params` = Form + QueryString + Cookies + ServerVariables
    - Parameter is `Stream`? Use `Request.InputStream`
    - Parameter is `IPrincipal`? Use `HttpContext.User`
    - Parameter is `HttpContext`? Use `HttpContext`
    - Parameter is `HttpContextWrapper`? Use `new HttpContextWrapper(context)`
    - Parameter is `JObject`? Use `JObject.Parse()`
    - Parameter any other type? Use `JsonDeserialize(model, parameterType)`
    - Model is `Null`? Use `default(T)`

## Result actions

- `JsonResult` - Returns data as JSON string. Is default result when method returns a non `BaseResult`
- `ErrorResult` - Returns an HTTP error in JSON format
- `FileContentResult` - Returns a file from a stream
- `FileResult` - Returns a file from disk
- `JsonPResult` - Returns JSON string with callback() function
- `TextResult` - Returns HTML data

## Authentication & authorization

- Use `[Authorize]` attribute - checks `HttpContext.User.Identity.IsAuthenticated`
- Use `[Role("admin", "user")]` attribute - checkes `HttpContext.User.IsInRole()`

# TODO

- Support for multiples MountPoints
- Permit path like `/download/{path*}`
