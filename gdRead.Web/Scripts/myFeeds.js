var gdRead = gdRead || {};
gdRead.app = angular.module("gdReadModule", ['ngSanitize', 'ui.bootstrap', 'infinite-scroll']);

gdRead.app.factory("feedService", ["$http", "$rootScope", "$timeout", function ($http, $rootScope, $timeout) {
    return {
        loadFeeds: function () {
            return $http.get("/Api/Feed");
        },
        loadPosts: function (feedId, page, filter) {
            return $http.get("/Api/Feed/" + feedId + "/Post/Page/" + page + "/Filter/" + filter);
        },
        loadAllPosts: function (page,filter) {
            return $http.get("/Api/Post/Page/" + page + "/Filter/" + filter);
        },
        addFeed: function (url) {
            return $http.post("/Api/Feed/", { Url: url });
        },
        markPostAsRead: function (post) {
            return $http.post("/Api/PostRead/", post);
        },
        markFeedAsRead: function (feed) {
            return $http.post("/Api/FeedRead/", feed);
        },
        loadPostContent: function (postId) {
            return $http.get("/Api/Post/ContentOnly/" + postId);
        },
        formatDate: function (longDate) {
            return longDate.replace("T", " ");
        },
        unsubscribe: function (feed) {
            return $http.delete("/Api/Feed/" + feed.Id);
        },
        focus: function (name) {
            $timeout(function () {
                $rootScope.$broadcast('focusOn', name);
            });
        }
    };
}]);

gdRead.app.directive('focusOn', function () {
    return function (scope, elem, attr) {
        scope.$on('focusOn', function (e, name) {
            if (name === attr.focusOn) {
                elem[0].focus();
            }
        });
    };
});

gdRead.app.controller("myFeedCtrl", ["$scope", "feedService", "$modal", "$timeout", "$sce",'$anchorScroll', '$location', function ($scope, feedService, $modal, $timeout, $sce, $anchorScroll, $location) {
    //Scope Methods
    $scope.loadNextPage = function () {
        var postFeedRequest;
        if ($scope.currentFeed.Id != null)
            postFeedRequest = feedService.loadPosts($scope.currentFeed.Id, $scope.currentPage, $scope.currentFilter);
        else postFeedRequest = feedService.loadAllPosts($scope.currentPage, $scope.currentFilter);
        postFeedRequest.success(function (posts) {
            $scope.currentPage++;
            if (!$scope.currentPosts)
                $scope.currentPosts = [];
            for (var i = 0; i < posts.length; i++) {
                posts[i].PublishDate = feedService.formatDate(posts[i].PublishDate);
                $scope.currentPosts.push(posts[i]);
            }
        });
    };

    $scope.currentFilter = "all";

    $scope.filter = function(type) {
        $scope.currentFilter = type;        
        $scope.currentPosts = null;
        var feedRequest = feedService.loadFeeds();
        feedRequest.success(function (feeds) {
            $scope.feeds = feeds;
            $scope.feedsLoading = false;
            $scope.selectAllFeeds();
        });
    };

    $scope.selectAllFeeds = function () {
        console.log("Selected all feeds")
        $scope.currentFeed = { Title: "All Feeds" };
        $scope.currentPage = 1;
        $scope.currentPosts = null;
        if ($scope.feeds) {
            for (var i = 0; i < $scope.feeds.length; i++) {
                $scope.feeds[i].selected = false;
            }
        }
        $scope.loadNextPage();
    };

    $scope.feedSelected = function (feed) {
        $scope.currentPage = 1;
        $scope.currentPosts = null;
        if (!feed) feed = $scope.comboSelectedFeed;
        for (var i = 0; i < $scope.feeds.length; i++) {
            $scope.feeds[i].selected = false;
        }
        $scope.currentFeed = feed;
        feed.selected = true;

        $scope.loadNextPage();
    };

    $scope.markFeedAsRead = function (feed) {
        var result = feedService.markFeedAsRead(feed);
        result.success(function () {
            $scope.feedSelected(feed);
            for (var i = 0; i < $scope.feeds.length; i++)
                if ($scope.feeds[i].Id === feed.Id)
                    $scope.feeds[i].UnreadCount = 0;
        });
    };

    $scope.unsubscribe = function (feed) {
        var result = feedService.unsubscribe(feed);
        result.success(function () {
            $scope.currentFeed = { Title: "All Feeds" };
            $scope.currentPosts = null;
            for (var i = 0; i < $scope.feeds.length; i++) {
                if ($scope.feeds[i].Id === feed.Id)
                    $scope.feeds.splice(i, 1);
            }
        });
    };

    $scope.selectPost = function (post) {
        if (!post.Selected) {
            post.Selected = true;


            if (!post.Content || post.Content === "") {
                var loadPostContent = feedService.loadPostContent(post.Id);
                post.Loading = true;
                loadPostContent.success(function (data) {
                    post.Loading = false;
                    post.Content = data.Content;
                });
            }

            if (!post.Read) {
                var result = feedService.markPostAsRead(post);
                result.success(function () {
                    post.Read = true;
                    for (var i = 0; i < $scope.feeds.length; i++) {
                        if ($scope.feeds[i].Id === post.FeedId) {
                            $scope.feeds[i].UnreadCount -= 1;
                        }
                    }
                });
            }

        } else {
            post.Selected = false;
        }
    };

    $scope.minimizeMaximiseFeedList = function () {
        $scope.feedsExpanded = !$scope.feedsExpanded;
    };

    $scope.openAddFeedModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addFeedWindow.html',
            controller: "addFeedModalCtrl"
        });
        $timeout(function () { feedService.focus("addFeedFocus"); }, 0);
        modalInstance.result.then(function () {
            feedRequest = feedService.loadFeeds();
            feedRequest.success(function (feeds) {
                $scope.feeds = feeds;
            });
        });
    };

    //Init Scope
    $scope.feedsLoading = true;
    $scope.feedsExpanded = true;
    $scope.currentFeed = { Title: "All Feeds" };
    $scope.currentPage = 1;

    //Load Feeds
    var feedRequest = feedService.loadFeeds();
    feedRequest.success(function (feeds) {
        $scope.feeds = feeds;
        $scope.feedsLoading = false;
    });
}]);



gdRead.app.controller("addFeedModalCtrl", ["$scope", "$modalInstance", "feedService", function ($scope, $modalInstance, feedService) {
    $scope.feedLoading = false;
    $scope.modelValues = {};
    $scope.ok = function () {
        $scope.feedLoading = true;
        var apiReturn = feedService.addFeed($scope.modelValues.url);
        apiReturn.success(function () {
            $scope.feedLoading = false;
            $modalInstance.close();
        });


    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}]);