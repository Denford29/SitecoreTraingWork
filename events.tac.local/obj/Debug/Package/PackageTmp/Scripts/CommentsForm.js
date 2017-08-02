function createCommentItem(form, path)
{
    var currentDate = new Date();
    var service = new ItemService({ url: '/sitecore/api/ssc/item' });
    var commentItem = {
        ItemName: form.name.value + '-comment-' + currentDate.getTime(),
        TemplateID: '{DBE79748-8E29-4C11-B7A6-8D47014E851C}',
        Name: form.name.value,
        Comment: form.comment.value
    };

    service.create(commentItem)
    .path(path)
    .execute()
    .then(function (item) {
        form.name.value = form.comment.value = '';
        window.alert('Thanks, your message will show on the site shortly');
    })
    .fail(function (error) {
        window.alert(error);
    });

    event.preventDefault();

    return false;
}