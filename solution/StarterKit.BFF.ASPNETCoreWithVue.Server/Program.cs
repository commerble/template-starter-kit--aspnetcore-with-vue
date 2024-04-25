using StarterKit.BFF.ASPNETCoreWithVue.Server.Commerble;

/**
 * SPAとASP.NET Viewsを組み合わせるときのキーポイント
 * 1. Properties/launchSettings.jsonのASPNETCORE_HOSTINGSTARTUPASSEMBLIESを削除する
 *    csprojのSpaProxyLaunchCommandとSpaProxyServerUrlは削除してよい
 * 2. スタートアッププロジェクトは、ServerとClientのマルチスタートアッププロジェクトを構成する
 * 3. ASP.NET Viewsでルート(/)を表示させるときは、`app.UseDefaultFiles()`を削除する
 * 4. AddControllersWithViewsとUseRouting、MapControllerRouteでスタートアップを組む
 */

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Proxy").Get<ProxySettings>() ?? throw new ApplicationException();

builder.Services.AddReverseProxy().AddCommerbleProxy(settings);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// app.UseDefaultFiles();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapReverseProxy();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("/index.html");

app.Run();
