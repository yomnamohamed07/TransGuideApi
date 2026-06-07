using Microsoft.AspNetCore.SignalR;
using TransGuide.Data.MappingProfiles.Inputs;

public class SignHub : Hub
{
    private readonly AiService _ai;
    private readonly SignSessionService _session;

    public SignHub(AiService ai, SignSessionService session)
    {
        _ai = ai;
        _session = session;
    }

    // join session group
    public async Task StartSession(Guid sessionId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            sessionId.ToString()
        );
    }

    // live frames
    public async Task SendLandmarks(LandmarkRequest request)
    {
        try
        {
            var prediction = await _ai.PredictAsync(
                request.session_id,
                request.landmarks
            );

            // update heartbeat
            await _session.TouchAsync(request.session_id);

            // send result to frontend
            await Clients.Caller.SendAsync("prediction", new
            {
                character = prediction.prediction,
                label = prediction.label,
                confidence = prediction.confidence
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("error", ex.Message);
        }
    }
}