using Microsoft.AspNetCore.Mvc;
using SIRI.Models;
using System.Linq;
using System;
using Microsoft.Extensions.Options;

namespace SIRI.Controllers
{

    [Route("siri/server/2.0")]

    public class SiriServerController : Controller
    {

        private readonly SiriSystemInfo _info;

        public SiriServerController(IOptions<SiriSystemInfo> optionsAccessor)
        {
            _info = optionsAccessor.Value;
        }


        [HttpGet]
        public ActionResult Get()
        {
            return Ok(Repository.Siris.Values.ToArray());
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            if (Repository.Siris.ContainsKey(id))
            {
                return Ok(Repository.Siris[id]);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]

        public ActionResult Post([FromBody] Siri siri)
        {

            if (!this.ModelState.IsValid || siri == null)
            {
                return BadRequest();
            }
            else
            {
                var maxExistingId = 0;
                if (Repository.Siris.Count > 0)
                {
                    maxExistingId = Repository.Siris.Keys.Max();

                }
                //                siri.ID = maxExistingId + 1;
                Repository.Siris.Add(maxExistingId + 1, siri);

                SiriServiceInformation serviceInfo = new SiriServiceInformation();
                serviceInfo.ServiceStartTime = _info.ServiceStartTime;

                SiriManager sm = new SiriManager(siri, serviceInfo);
                sm.process();
                switch (sm.status)
                {
                    case SiriManager.Status.ValidAndResponse:
                        return Ok(sm.response);

                    case SiriManager.Status.ValidNoResponse:
                        return Accepted();

                    case SiriManager.Status.ValidNotSupport:
                        return UnprocessableEntity();

                    default:
                        return BadRequest();
                }
            }

        }
    }



}