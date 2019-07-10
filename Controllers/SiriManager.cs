using System;
using System.Collections.Generic;
using System.Linq;
using SIRI.Models;

namespace SIRI.Controllers
{
    public class SiriServiceInformation
    {
        public DateTime ServiceStartTime { get; set; }
    }






    public class SiriManager
    {
        public enum Status
        {
            ValidAndResponse,
            ValidNoResponse,
            ValidNotSupport,
            Invalid
        }

        private Siri siri;

        private SiriServiceInformation serviceInfo;

        public Siri response { get; private set; }

        public Status status { get; private set; }

        public SiriManager(Siri siriInput, SiriServiceInformation serviceInfoInput)
        {
            this.siri = siriInput;
            this.serviceInfo = serviceInfoInput;
        }

        //create response Siri message object
        public void process()
        {

            string siriType = siri.Item.GetType().ToString();
            //System.Console.WriteLine(siriType);
            switch (siriType)
            {
                //valid and response
                case "SIRI.Models.CheckStatusRequestStructure":
                    System.Console.WriteLine("CheckStatusRequest");
                    createCheckStatusResponse();
                    status = Status.ValidAndResponse;
                    break;

                case "SIRI.Models.SubscriptionRequest":
                    System.Console.WriteLine("SubscriptionRequest");
                    createSubscriptionResponse();
                    status = Status.ValidAndResponse;
                    break;

                case "SIRI.Models.TerminateSubscriptionRequestStructure":
                    System.Console.WriteLine("TerminateSubscriptionRequest");
                    status = Status.ValidAndResponse;
                    break;

                case "SIRI.Models.ServiceDelivery":
                    System.Console.WriteLine("ServiceDelivery");
                    status = Status.ValidAndResponse;
                    break;

                //valid but no response
                case "SIRI.Models.HeartbeatNotificationStructure":
                    System.Console.WriteLine("Heartbeat");
                    status = Status.ValidNoResponse;
                    break;

                case "SIRI.Models.SubscriptionResponseStructure":
                    System.Console.WriteLine("SubscriptionResponse");
                    status = Status.ValidNoResponse;
                    break;

                case "SIRI.Models.SubscriptionTerminatedNotificationStructure":
                    System.Console.WriteLine("SubscriptionTerminated");
                    status = Status.ValidNoResponse;
                    break;

                case "SIRI.Models.CheckStatusResponseStructure":
                    System.Console.WriteLine("CheckStatusResponse");
                    status = Status.ValidNoResponse;
                    break;

                case "SIRI.Models.DataReceivedResponseStructure":
                    System.Console.WriteLine("DataReceivedResponse");
                    status = Status.ValidNoResponse;
                    break;

                case "SIRI.Models.TerminateSubscriptionResponseStructure":
                    System.Console.WriteLine("TerminateSubscriptionResponse");
                    status = Status.ValidNoResponse;
                    break;

                default:
                    status = Status.ValidNotSupport;
                    break;
            }//switch
        }//porcess


        // create check status response
        private void createCheckStatusResponse()
        {
            Siri responseSiri = new Siri();
            CheckStatusRequestStructure checkStatusRequest = (CheckStatusRequestStructure)siri.Item;

            CheckStatusResponseStructure csr = new CheckStatusResponseStructure();
            csr.ResponseTimestamp = DateTime.Now;
            csr.Status = true;
            csr.ServiceStartedTime = serviceInfo.ServiceStartTime;
            if (checkStatusRequest.MessageIdentifier != null)
            {
                MessageRefStructure requestMessageRef = new MessageRefStructure();
                requestMessageRef.Value = checkStatusRequest.MessageIdentifier.Value;
                csr.RequestMessageRef = requestMessageRef;
            }
            responseSiri.Item = csr;
            this.response = responseSiri;

        }//createCheckStatusResponse



        // create subsription response
        private void createSubscriptionResponse()
        {
            Siri responseSiri = new Siri();
            SubscriptionResponseStructure srs = new SubscriptionResponseStructure();
            srs.ResponseTimestamp = DateTime.Now;
            ParticipantRefStructure responder = new ParticipantRefStructure();
            responder.Value = "TfNSW_SIRI_Service";
            srs.ResponderRef = responder;
            List<StatusResponseStructure> responseStatus = new List<StatusResponseStructure>();



            //identify the subscription information
            SubscriptionRequest sr = (SubscriptionRequest)siri.Item;
            //esitmated timetable susbcription request
            if (sr.EstimatedTimetableSubscriptionRequest != null)
            {

                for (int i = 0; i < sr.EstimatedTimetableSubscriptionRequest.Length; i++)
                {
                    responseStatus.Add(createSubscriptionResponseStatus(sr.EstimatedTimetableSubscriptionRequest[i]));
                }

            }

            //situation exchange subscription request
            if (sr.SituationExchangeSubscriptionRequest != null)
            {

                for (int i = 0; i < sr.SituationExchangeSubscriptionRequest.Length; i++)
                {
                    responseStatus.Add(createSubscriptionResponseStatus(sr.SituationExchangeSubscriptionRequest[i]));
                }

            }

            //vehicle monitoring subscription request
            if (sr.VehicleMonitoringSubscriptionRequest != null)
            {

                for (int i = 0; i < sr.VehicleMonitoringSubscriptionRequest.Length; i++)
                {
                    responseStatus.Add(createSubscriptionResponseStatus(sr.VehicleMonitoringSubscriptionRequest[i]));
                }

            }


            srs.ResponseStatus = responseStatus.ToArray();
            responseSiri.Item = srs;
            this.response = responseSiri;

        }//createSubscriptionResponse


        public StatusResponseStructure createSubscriptionResponseStatus(AbstractSubscriptionStructure inputSubscription)
        {
            int maxExistingId = 0;
            if (Repository.Subscriptions.Count > 0)
            {
                maxExistingId = Repository.Subscriptions.Keys.Max();

            }

            SiriSubscription siriSubscription = new SiriSubscription();
            siriSubscription.subscription = (AbstractSubscriptionStructure)inputSubscription;

            StatusResponseStructure statusResponse = new StatusResponseStructure();
            statusResponse.ResponseTimestamp = DateTime.Now;
            statusResponse.SubscriberRef = inputSubscription.SubscriberRef;
            statusResponse.SubscriptionRef = inputSubscription.SubscriptionIdentifier;
            statusResponse.Status = true;
            statusResponse.ShortestPossibleCycle = null;
            siriSubscription.subscriptionResponse = statusResponse;
            Repository.Subscriptions.Add(maxExistingId + 1, siriSubscription);
            return statusResponse;

        }//createSubscriptionResponseStatus






    }//siriManager

}//namespace