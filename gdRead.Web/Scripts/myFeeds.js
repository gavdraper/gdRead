﻿var gdRead = gdRead || {};
gdRead.app = angular.module("gdReadModule", ['ngSanitize', 'ui.bootstrap']);

gdRead.app.factory("feedService", function ($http, $rootScope, $timeout) {
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
            return $http.post("/Api/Feeds/", { Url: url });
        },
        markPostAsRead:function(post) {
            return $http.post("/Api/PostRead/", post);
        },
        formatDate: function (longDate) {
            return longDate.replace("T", " ");
        },
        focus: function (name) {
            $timeout(function () {
                $rootScope.$broadcast('focusOn', name);
            });
        }
    };
});

gdRead.app.directive('focusOn', function () {
    return function (scope, elem, attr) {
        scope.$on('focusOn', function (e, name) {            
            if (name === attr.focusOn) {
                elem[0].focus();                 
            }
        });
    };
});

gdRead.app.controller("myFeedCtrl", function ($scope, feedService, $modal, $timeout) {
    $scope.feedsLoading = true;
    var feedRequest = feedService.loadFeeds();
    feedRequest.success(function (feeds) {
        $scope.feeds = feeds;
        $scope.feedsLoading = false;
    });

    $scope.selectAllFeeds = function () {
        $scope.currentFeed = { title: "All Feeds" };
    };

    $scope.feedSelected = function (feed) {
        if (!feed) feed = $scope.comboSelectedFeed;
        $scope.currentFeed.selected = false;
        $scope.currentFeed = { title: feed.Title };
        feed.selected = true;
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
            if (!post.Read) {
                var result = feedService.markPostAsRead(post);
                
                result.success(function() {
                    post.Read = true; 
                    for (var i = 0; i < $scope.feeds.length; i++) {
                        if ($scope.feeds[i].Id == post.FeedId) {
                            $scope.feeds[i].UnreadCount -= 1;
                        }
                    }

                });

            }
        } else {
            post.Selected = false;
        }
    };

    $scope.openAddFeedModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addFeedWindow.html',
            controller: addFeedModalCtrl
        });

        $timeout(function () { feedService.focus("addFeedFocus"); }, 0);
        
        modalInstance.result.then(function () {
            feedRequest = feedService.loadFeeds();
            feedRequest.success(function (feeds) {
                $scope.feeds = feeds;
            });
        });
    };

    $scope.selectAllFeeds();
});


var addFeedModalCtrl = function ($scope, $modalInstance, feedService) {
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
};