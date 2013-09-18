using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.SessionState;
using Badges.Core.Repositories;
using UCDArch.Web.ActionResults;

namespace Badges.Controllers
{
    /// <summary>
    /// Controller for the Assertion class
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)] //Don't track session with these jason assertions
    public class AssertionController : ApplicationController
    {
        public AssertionController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        /// <summary>
        /// returns information about a specific badge awarded to a specific user
        /// </summary>
        /// <see cref="https://github.com/mozilla/openbadges/wiki/Assertions"/>
        /// <param name="id">Id of badge submission/award</param>
        /// <returns>Badge Assertion</returns>
        public ActionResult UserBadge(Guid id)
        {
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null || badgeSubmission.Approved == false) return HttpNotFound();

            var email = badgeSubmission.Creator.Profile.Email;

            var recipient = new
            {
                type = "email",
                hashed = true,
                identity = "sha256$" + HashString(email),
            };

            var verify = new { type = "hosted", url = AbsoluteUrl("userbadge", id: id) };

            var obj = new
            {
                badgeSubmission.Id,
                recipient,
                image = badgeSubmission.Badge.ImageUrl,
                evidence = AbsoluteUrl("Badge", "Public", id),
                issuedOn = GetUnixTime(badgeSubmission.AwardedOn),
                badge = AbsoluteUrl("Badge", id: badgeSubmission.Badge.Id),
                verify
            };

            return new JsonNetResult(obj);
        }

        /// <summary>
        /// Returns info about a badge itself
        /// </summary>
        /// <param name="id">locally unique id of the badge</param>
        /// <returns></returns>
        public ActionResult Badge(Guid id)
        {
            //          alignment": [
            //  { "name": "CCSS.ELA-Literacy.RST.11-12.3",
            //    "url": "http://www.corestandards.org/ELA-Literacy/RST/11-12/3",
            //    "description": "Follow precisely a complex multistep procedure when carrying out experiments, taking measurements, or performing technical tasks; analyze the specific results based on explanations in the text."
            //  },
            //  { "name": "CCSS.ELA-Literacy.RST.11-12.9",
            //    "url": "http://www.corestandards.org/ELA-Literacy/RST/11-12/9",
            //    "description": " Synthesize information from a range of sources (e.g., texts, experiments, simulations) into a coherent understanding of a process, phenomenon, or concept, resolving conflicting information when possible."
            //  }
            //]

            var badge = RepositoryFactory.BadgeRepository.GetNullableById(id);

            if (badge == null) return HttpNotFound();

            var obj = new
            {
                name = badge.Name,
                description = badge.Description,
                image = badge.ImageUrl,
                criteria = AbsoluteUrl("Criteria", "Public", id: id),
                tags = new[] { "ucdbadges" },
                issuer = AbsoluteUrl("Organization"),
            };

            return new JsonNetResult(obj);
        }

        /// <summary>
        /// Return information about who issued a badge
        /// </summary>
        /// <returns></returns>
        public ActionResult Organization()
        {
            var obj = new
            {
                name = "UC Davis Badges TEST",
                image = "http://asi.ucdavis.edu/img/logo-anr.jpg",
                url = "http://asi.ucdavis.edu/front-page",
                email = "fake@ucdavis.edu",
            };
            
            return new JsonNetResult(obj);
        }
        
        private static int GetUnixTime(DateTime? time)
        {  
            if (time.HasValue == false)
            {
                return default(int);
            }

            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan diff = time.Value.ToLocalTime() - origin;

            return (int)Math.Floor(diff.TotalSeconds);
        }

        private string HashString(string original)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(original);
            var hashstring = new SHA256Managed();
            var hash = hashstring.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + String.Format("{0:x2}", x));
        }

        private string AbsoluteUrl(string action, string controller = null, Guid? id = null)
        {
            return Url.Action(action, controller, new { area = string.Empty, id }, ControllerContext.HttpContext.Request.Url.Scheme);
        }
    }
}
