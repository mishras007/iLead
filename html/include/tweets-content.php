<div class="tweet-show-more">
    <div class="row">
        <div class="col-lg-4 col-sm-4" >
            <div class="text-center data-box bg_tweets_lightblue">
                <div class="tweet-icon"><i class="fa fa-twitter"></i></div>
                <span class="profile-image ">
                    <img src="img/a2.jpg">
                </span>
                <div class="text-center profile-name">@iledtheway</div>
                <div class="text-center twit-all-content profile-comment">
                    An LED Bulb works for 50,000 hours.A normal bulb may fuse after 1200! <br/>#ILEDTHEWAY
                </div> 
            </div>
        </div>
        <div class="col-lg-4 col-sm-4" >
            <div class="text-center data-box bg_tweets_darkblue">
                <div class="tweet-icon"><i class="fa fa-twitter"></i></div>
                <span class="profile-image ">
                    <img src="img/a2.jpg">
                </span>
                <div class="text-center profile-name">@lalseth</div>
                <div class="text-center twit-all-content profile-comment">
                   300 Rupees saved per LED Bulb! Great initiative from the government! <br/>#ILEDTHEWAY
                </div> 
            </div>
        </div>
        <div class="col-lg-4 col-sm-4" >
            <div class="text-center data-box bg_tweets_lightblue">
                <div class="tweet-icon"><i class="fa fa-twitter"></i></div>
                <span class="profile-image ">
                    <img src="img/a2.jpg">
                </span>
                <div class="text-center profile-name">@Budtt</div>
                <div class="text-center twit-all-content profile-comment">
                    The smartest solution to lower your monthly electric bill!<br/>#ILEDTHEWAY
                </div> 
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-4 col-sm-4" >
            <div class="text-center data-box bg_tweets_darkblue">
                <div class="tweet-icon"><i class="fa fa-twitter"></i></div>
                <span class="profile-image ">
                    <img src="img/a2.jpg">
                </span>
                <div class="text-center profile-name">@iledtheway</div>
                <div class="text-center twit-all-content profile-comment">
                    An LED Bulb works for 50,000 hours.A normal bulb may fuse after 1200! <br/>#ILEDTHEWAY
                </div> 
            </div>
        </div>
        <div class="col-lg-4 col-sm-4" >
            <div class="text-center data-box bg_tweets_lightblue">
                <div class="tweet-icon"><i class="fa fa-twitter"></i></div>
                <span class="profile-image ">
                    <img src="img/a2.jpg">
                </span>
                <div class="text-center profile-name">@lalseth</div>
                <div class="text-center twit-all-content profile-comment">
                   300 Rupees saved per LED Bulb! Great initiative from the government! <br/>#ILEDTHEWAY
                </div> 
            </div>
        </div>
        <div class="col-lg-4 col-sm-4" >
            <div class="text-center data-box bg_tweets_darkblue">
                <div class="tweet-icon"><i class="fa fa-twitter"></i></div>
                <span class="profile-image ">
                    <img src="img/a2.jpg">
                </span>
                <div class="text-center profile-name">@Budtt</div>
                <div class="text-center twit-all-content profile-comment">
                    The smartest solution to lower your monthly electric bill!<br/>#ILEDTHEWAY
                </div> 
            </div>
        </div>
    </div>
    <div id="loadingDiv"><img src="img/362.GIF"></div>
</div>
<div class="row">
    <div class="col-lg-12 col-sm-12" >
        <div class="load-more">
            <a id="tweet-more" href="javascript:void(0);">load more</a>
        </div>
    </div>
</div>


    <script>
    $(function(){
        $('#tweet-more').on( "click", function() {
            $('#loadingDiv').fadeIn(100);
            $.ajax({
                url: "ajax/tweets.html",
                success: function (data) { 
                    $('#loadingDiv').fadeOut(500);
                    $('.tweet-show-more').append(data);
                     },
                dataType: 'html'
                

            });
  
        });


    });
     </script>        