// Code generated by Microsoft (R) AutoRest Code Generator 0.13.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using System.Text.RegularExpressions;

namespace Microsoft.Bot.Connector
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json.Linq;
    using System.Net.Http;
    using System.Configuration;
    using System.Text;

    public partial class Activity : IActivity, IContactRelationUpdateActivity, IMessageActivity, ITypingActivity, IConversationUpdateActivity, IActionActivity
    {
        [JsonExtensionData(ReadData = true, WriteData = true)]
        public JObject Properties { get; set; }

        /// <summary>
        /// Take a message and create a reply message for it with the routing information 
        /// set up to correctly route a reply to the source message
        /// </summary>
        /// <param name="text">text you want to reply with</param>
        /// <param name="locale">language of your reply</param>
        /// <returns>message set up to route back to the sender</returns>
        public Activity CreateReply(string text = null, string locale = null)
        {
            Activity reply = new Activity();
            reply.Type = ActivityTypes.Message;
            reply.Timestamp = DateTime.UtcNow;
            reply.From = this.Recipient;
            reply.Recipient = this.From;
            reply.ReplyToId = this.Id;
            reply.ServiceUrl = this.ServiceUrl;
            reply.ChannelId = this.ChannelId;
            reply.Conversation = this.Conversation;
            reply.Text = text ?? String.Empty;
            reply.Locale = locale ?? this.Locale;            
            reply.Attachments = new List<Attachment>();
            reply.Entities = new List<Entity>();
            return reply;
        }

        public static IMessageActivity CreateMessageActivity() { return new Activity(ActivityTypes.Message); }

        public static IContactRelationUpdateActivity CreateContactRelationUpdateActivity() { return new Activity(ActivityTypes.ContactRelationUpdate); }

        public static IConversationUpdateActivity CreateConversationUpdateActivity() { return new Activity(ActivityTypes.ConversationUpdate); }

        public static ITypingActivity CreateTypingActivity() { return new Activity(ActivityTypes.Typing); }

        /// <summary>
        /// Get StateClient appropriate for this activity
        /// </summary>
        /// <param name="microsoftAppId"></param>
        /// <param name="microsoftAppPassword"></param>
        /// <param name="serviceUrl">alternate serviceurl to use for state service</param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public StateClient GetStateClient(string microsoftAppId = null, string microsoftAppPassword = null, string serviceUrl = null, params DelegatingHandler[] handlers)
        {
            bool useServiceUrl = (this.ChannelId == "emulator" || (this.ChannelId.StartsWith("skype") && this.ChannelId.Length > "skype".Length));
            if (useServiceUrl)
                return new StateClient(new Uri(this.ServiceUrl), microsoftAppId, microsoftAppPassword, handlers);

            if (serviceUrl != null)
                return new StateClient(new Uri(serviceUrl), microsoftAppId, microsoftAppPassword, handlers);

            return new StateClient(new MicrosoftAppCredentials(microsoftAppId, microsoftAppPassword), true, handlers);
        }

        /// <summary>
        /// Check if the message has content
        /// </summary>
        /// <returns>Returns true if this message has any content to send</returns>
        public bool HasContent()
        {
            if (!String.IsNullOrWhiteSpace(this.Text))
                return true;

            if (!String.IsNullOrWhiteSpace(this.Summary))
                return true;

            if (this.Attachments != null && this.Attachments.Any())
                return true;

            if (this.ChannelData != null)
                return true;

            return false;
        }

        /// <summary>
        /// Get mentions 
        /// </summary>
        /// <returns></returns>
        public Mention[] GetMentions()
        {
            return this.Entities?.Where(entity => String.Compare(entity.Type, "mention", ignoreCase: true) == 0).Select(e => e.Properties.ToObject<Mention>()).ToArray() ?? new Mention[0];
        }

        /// <summary>
        /// Is there a mention of Id in the Text Property 
        /// </summary>
        /// <param name="id">ChannelAccount.Id</param>
        /// <returns>true if this id is mentioned in the text</returns>
        public bool MentionsId(string id)
        {
            return this.GetMentions().Where(mention => mention.Mentioned.Id == id).Any();
        }

        /// <summary>
        /// Is there a mention of Recipient.Id in the Text Property 
        /// </summary>
        /// <returns>true if this id is mentioned in the text</returns>
        public bool MentionsRecipient()
        {
            return this.GetMentions().Where(mention => mention.Mentioned.Id == this.Recipient.Id).Any();
        }

        /// <summary>
        /// Remove recipient mention text from Text property
        /// </summary>
        /// <returns>new .Text property value</returns>
        public string RemoveRecipientMention()
        {
            return RemoveMentionText(this.Recipient.Id);
        }

        /// <summary>
        /// Replace any mention text for given id from Text property
        /// </summary>
        /// <param name="id">id to match</param>
        /// <returns>new .Text property value</returns>
        public string RemoveMentionText(string id)
        {
            foreach (var mention in this.GetMentions().Where(mention => mention.Mentioned.Id == id))
            {
                Text = Regex.Replace(Text, mention.Text, "", RegexOptions.IgnoreCase);
            }
            return this.Text;
        }

        /// <summary>
        /// Get channeldata as typed structure
        /// </summary>
        /// <typeparam name="TypeT">type to use</typeparam>
        /// <returns>typed object or default(TypeT)</returns>
        public TypeT GetChannelData<TypeT>()
        {
            if (this.ChannelData == null)
                return default(TypeT);
            return ((JObject)this.ChannelData).ToObject<TypeT>();
        }

        /// <summary>
        /// Return the "major" portion of the activity
        /// </summary>
        /// <returns>normalized major portion of the activity, aka message/... will return "message"</returns>
        public string GetActivityType()
        {
            var type = this.Type.Split('/').First();
            return GetActivityType(type);
        }

        public static string GetActivityType(string type)
        {
            if (String.Equals(type, ActivityTypes.Message, StringComparison.OrdinalIgnoreCase))
                return ActivityTypes.Message;

            if (String.Equals(type, ActivityTypes.ContactRelationUpdate, StringComparison.OrdinalIgnoreCase))
                return ActivityTypes.ContactRelationUpdate;

            if (String.Equals(type, ActivityTypes.ConversationUpdate, StringComparison.OrdinalIgnoreCase))
                return ActivityTypes.ConversationUpdate;

            if (String.Equals(type, ActivityTypes.DeleteUserData, StringComparison.OrdinalIgnoreCase))
                return ActivityTypes.DeleteUserData;

            if (String.Equals(type, ActivityTypes.Typing, StringComparison.OrdinalIgnoreCase))
                return ActivityTypes.Typing;

            if (String.Equals(type, ActivityTypes.Ping, StringComparison.OrdinalIgnoreCase))
                return ActivityTypes.Ping;

            return $"{Char.ToLower(type[0])}{type.Substring(1)}";
        }
    }
}
