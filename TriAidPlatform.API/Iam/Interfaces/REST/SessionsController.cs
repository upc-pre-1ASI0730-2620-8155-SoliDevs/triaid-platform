using System.Collections.Concurrent;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Iam.Interfaces.Rest;

/**
 * <summary>
 *     The sessions controller
 * </summary>
 * <remarks>
 *     Tracks open web sessions in memory. The web client opens a
 *     session after matching credentials against the staff listing
 *     and closes it on sign out (ADR-010 backend swap contract).
 * </remarks>
 */
[ApiController]
[Route("sessions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Sessions")]
public class SessionsController : ControllerBase
{
    private static readonly ConcurrentDictionary<int, SessionResource> Sessions = new();
    private static int _nextId;

    /**
     * <summary>
     *     List every open session
     * </summary>
     * <returns>The open sessions</returns>
     * <response code="200">Open sessions</response>
     */
    [HttpGet]
    [EndpointSummary("List open sessions")]
    public IActionResult GetAll()
    {
        return Ok(Sessions.Values.OrderBy(s => s.Id));
    }

    /**
     * <summary>
     *     Open a session for a signed-in staff account
     * </summary>
     * <param name="resource">The session data sent by the client</param>
     * <returns>The created session with its identifier</returns>
     * <response code="200">Session opened</response>
     */
    [HttpPost]
    [EndpointSummary("Open a session")]
    public IActionResult Open([FromBody] OpenSessionResource resource)
    {
        var id = Interlocked.Increment(ref _nextId);
        var session = new SessionResource(id, resource.UserId, resource.Username, resource.OpenedAt);
        Sessions[id] = session;
        return Ok(session);
    }

    /**
     * <summary>
     *     Close a session
     * </summary>
     * <param name="id">The session identifier</param>
     * <returns>An empty result</returns>
     * <response code="200">Session closed</response>
     * <response code="404">Session not found</response>
     */
    [HttpDelete("{id:int}")]
    [EndpointSummary("Close a session")]
    public IActionResult Close(int id)
    {
        return Sessions.TryRemove(id, out _) ? Ok(new { closed = id }) : NotFound(new { error = "session-not-found" });
    }
}
