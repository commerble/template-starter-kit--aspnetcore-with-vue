using StarterKit.BFF.ASPNETCoreWithVue.Server.Commerble;

/**
 * SPA��ASP.NET Views��g�ݍ��킹��Ƃ��̃L�[�|�C���g
 * 1. Properties/launchSettings.json��ASPNETCORE_HOSTINGSTARTUPASSEMBLIES���폜����
 *    csproj��SpaProxyLaunchCommand��SpaProxyServerUrl�͍폜���Ă悢
 * 2. �X�^�[�g�A�b�v�v���W�F�N�g�́AServer��Client�̃}���`�X�^�[�g�A�b�v�v���W�F�N�g���\������
 * 3. ASP.NET Views�Ń��[�g(/)��\��������Ƃ��́A`app.UseDefaultFiles()`���폜����
 * 4. AddControllersWithViews��UseRouting�AMapControllerRoute�ŃX�^�[�g�A�b�v��g��
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
