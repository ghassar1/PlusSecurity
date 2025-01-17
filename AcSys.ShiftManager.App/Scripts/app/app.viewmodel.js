﻿function AppViewModel(dataModel) {
    // Private state
    var self = this;

    // Private operations
    function cleanUpLocation() {
        window.location.hash = "";

        if (typeof (history.pushState) !== "undefined") {
            history.pushState("", document.title, location.pathname);
        }
    }
    // Data
    self.Views = {
        Loading: {} // Other views are added dynamically by app.addViewModel(...).
    };
    self.dataModel = dataModel;

    // UI state
    self.view = ko.observable(self.Views.Loading);

    self.loading = ko.computed(function () {
        return self.view() === self.Views.Loading;
    });

    // UI operations

    // Other navigateToX functions are added dynamically by app.addViewModel(...).

    // Other operations
    self.addViewModel = function (options) {
        var viewItem = new options.factory(self, dataModel),
            navigator;

        // Add view to AppViewModel.Views enum (for example, app.Views.Home).
        self.Views[options.name] = viewItem;

        // Add binding member to AppViewModel (for example, app.home);
        self[options.bindingMemberName] = ko.computed(function () {
            if (!dataModel.getAccessToken()) {
                // The following code looks for a fragment in the URL to get the access token which will be
                // used to call the protected Web API resource
                var fragment = common.getFragment();

                if (fragment.access_token) {
                    // returning with access token, restore old hash, or at least hide token
                    window.location.hash = fragment.state || '';
                    dataModel.setAccessToken(fragment.access_token);
                } else {
                    // no token - so bounce to Authorize endpoint in AccountController to sign in or register
                    window.location = "/Account/Authorize?client_id=web&response_type=token&state=" + encodeURIComponent(window.location.hash);
                }
            }

            return self.Views[options.name];
        });
        //http://localhost:64736/#access_token=5HWpambTdDfSq9fI45Dj5n0sLxuKRgsldobLv9iZfSfDh4s63xvUeVDuK9OVCslaFhTQuroTiOTsFgBtyVAVpCRdezlIB6gYaygLLzMGev3jtFu7K8BiVLdeR7Hu2ROtqUpky-pjhZMKO-vkJk8yhrUmMR0bgtBLQpD2psfwXf7BHLritMi7HmBwC-eskZutyGzqDzfgYs5AUJZsGFUkA37MSJF9fkVXJjBbxiSKyJGT8tjaxxedZd0MjLZGp5qG3DVxABKzfBAM0eSSI84X8FG6eE-Z0D3HzEtn9WpwkfxbppGaNX9hpquLB6z8WJHB7F0nKQSYQG-JQNMyW8MLSglKdj3SX-SLgVb-188Pon2ZjN8uoGEI_tlirjo06rh9QKa-wP8TZ_FmXYVTc9yc4IBRmkOzC7xonK27gWDuShdO8nxK--ZC4QLumm83CN3m0Btu60azNPxQ7UAEH765_joGrmk4gsm15ku-XcdhdwuMaylt9XICMHXrv__nrRji&token_type=bearer&expires_in=1209600
        if (typeof (options.navigatorFactory) !== "undefined") {
            navigator = options.navigatorFactory(self, dataModel);
        } else {
            navigator = function () {
                window.location.hash = options.bindingMemberName;
            };
        }

        // Add navigation member to AppViewModel (for example, app.NavigateToHome());
        self["navigateTo" + options.name] = navigator;
    };

    self.initialize = function () {
        Sammy().run();
    }
}

var app = new AppViewModel(new AppDataModel());
