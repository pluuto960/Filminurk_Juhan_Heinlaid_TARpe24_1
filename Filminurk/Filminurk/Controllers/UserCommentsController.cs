using Filminurk.Core.ServiceInterface;
using Filminurk.Data;
using Filminurk.Models.Comments;
using Microsoft.AspNetCore.Mvc;

namespace Filminurk.Controllers
{
    public class UserCommentsController : Controller
    {
        private readonly FilminurkTARpe24Context _context;
        private readonly IUserCommentsServices _userCommentsServices;

        public UserCommentsController(FilminurkTARpe24Context context, IUserCommentsServices userCommentsServices)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var result = _context.UserComments
                .Select(
                c => new UserCommentsIndexViewModel
                {
                    CommentID = c.CommentID,
                    CommentBody = c.CommentBody,
                    IsHarmful = c.IsHarmful,
                    CommentCreatedAt = c.CommentCreatedAt,
                    // CommentModifiedAt = c.CommentModifiedAt

                }
                );

            return View(result);
        }

        [HttpGet]
        public IActionResult NewComment()
        {
            // TODO: erista kas tegemist on admini voi tavakasutajaga
            UserCommentsCreateViewModel newcomment = new();
            return View(newcomment);
        }
        [HttpPost, ActionName("NewComment")]
        //meetodile ei tohi panna allowanonymous
        public async Task<IActionResult> NewCommentPost(UserCommentsCreateViewModel newcommentVM)
        {
            if (ModelState.IsValid) {
                var dto = new UserCommentDTO()
                {
                    CommentID = (Guid)newcommentVM.CommentID,
                    CommentBody = newcommentVM.CommentBody,
                    CommenterUserID = newcommentVM.CommenterUserID,
                    CommentedScore = newcommentVM.CommentedScore,
                    CommentCreatedAt = newcommentVM.CommentCreatedAt,
                    CommentModifiedAt = newcommentVM.CommentModifiedAt,
                    isHelpful = (int)newcommentVM.IsHelpful,
                    isHarmful = (int)newcommentVM.IsHarmful,
                };
                var result = await _userCommentsServices.NewComment(dto);
                if (result == null)
                {
                    return NotFound();
                }
                //TODO: Erista ära kas tegu kasutaja voi adminiga, admin tagastub admin-comments-indexi, kasutaja aga vastava filmi juurde
                return RedirectToAction(nameof(Index);
                //return RedirectToAction("Details", "Movies", id)
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> DetailsAdmin(Guid id)
        {
            var requestedComment = await _userCommentsServices.DetailAsync(id);

            if (requestedComment == null) { return NotFound(); }

            var commentVM = new UserCommentsIndexViewModel();


            commentVM.CommentID = requestedComment.CommentID;
            commentVM.CommentBody = requestedComment.CommentBody;
            commentVM.CommentUserID = requestedComment.CommentUserID;
            commentVM.CommentScore = requestedComment.CommentScore;
            commentVM.CommentCreatedAt = requestedComment.CommentCreatedAt;
            commentVM.CommentModifiedAt = requestedComment.CommentModifiedAt;
            commentVM.CommentDeletedAt = requestedComment.CommentDeletedAt;

            return View(commentVM);
        }

    }
}