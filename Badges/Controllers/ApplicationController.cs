using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace Badges.Controllers
{
    [Version(MajorVersion = 3)]
    //[ServiceMessage("Badges", ViewDataKey = "ServiceMessages", MessageServiceAppSettingsKey = "MessageService")]
    public abstract class ApplicationController : SuperController
    {
    }
}