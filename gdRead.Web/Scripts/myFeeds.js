var gdRead = gdRead || {};
gdRead.app = angular.module("gdReadModule", ['ngSanitize', 'ui.bootstrap']);

gdRead.app.factory("feedService", function ($http) {
    return {
        loadFeeds: function () {
            return $http.get("/Api/Feeds");
        },
        loadPosts: function (feedId) {
            return $http.get("/Api/Posts/" + feedId);
        },
        loadAllPosts: function () {
            return $http.get("/Api/Posts/");
        },
        addFeed: function (url) {
            console.log(url);
            return $http.post("/Api/Feeds/", { Url: url });
        },
        formatDate: function (longDate) {
            return longDate.replace("T", " ");
        }
    };
});

gdRead.app.controller("myFeedCtrl", function ($scope, feedService, $modal) {

    var feedRequest = feedService.loadFeeds();
    feedRequest.success(function (feeds) {
        $scope.feeds = feeds;
    });

    $scope.selectAllFeeds = function () {
        $scope.currentFeed = { title: "All Feeds" };
    };

    $scope.feedSelected = function (feed) {
        $scope.currentFeed = { title: feed.Title };
        var postFeedRequest = feedService.loadPosts(feed.Id);
        postFeedRequest.success(function (posts) {
            for (var i = 0; i < posts.length; i++)
                posts[i].PublishDate = feedService.formatDate(posts[i].PublishDate);
            $scope.currentFeed.Posts = posts;
        });
    };

    $scope.selectPost = function (post) {
        if (!post.Selected) {
            post.Selected = true;
        } else {
            post.Selected = false;
        }
    };

    $scope.openAddFeedModal = function () {
        $modal.open({
            templateUrl: 'addFeedWindow.html',
            controller: addFeedModalCtrl
        });
    };

    $scope.selectAllFeeds();
});


var addFeedModalCtrl = function ($scope, $modalInstance, feedService) {

    $scope.modelValues = {};
    $scope.ok = function () {
        feedService.addFeed($scope.modelValues.url);
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
};