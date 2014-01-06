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