<div class="facts-show-more">
                <div class="row">
                    <div class="col-md-4">
                        <div class="text-center facts-data-box bg_tweets_lightblue">
                            <div class="inner-div">
                            <span class="image-wrap">
                                <img src="img/white-bulb.png" class="image_top ">
                            </span>
                            <div class="text-center twit-all-content facts-content">save<span class="number-font"> 66% </span>on LED purchase</div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="text-center facts-data-box bg_facts_grey">
                            <div class="inner-div">
                                <span >
                                    <img src="img/watch.png" class="image_top ">
                                </span>
                                <div class="text-center twit-all-content facts-content_blue">
                                    LEDs can be dimmed allowing the amount of light to be controlled.
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="text-center facts-data-box bg_tweets_lightblue">
                            <div class="inner-div">
                            <span class="image-wrap">
                                <img src="img/white-bulb.png" class="image_top ">
                            </span>
                            <div class="text-center twit-all-content facts-content">save<span class="number-font"> 66% </span>on LED purchase</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-4">
                        <div class="text-center facts-data-box bg_facts_grey">
                            <div class="inner-div">
                                <span class="image-wrap">
                                    <img src="img/blue-bulb.png" class="image_top ">
                                </span>
                                <div class="text-center twit-all-content facts-content_blue">save<span class="yellow-font"> 66% </span>on LED purchase
                                </div>
                            </div>
                        </div>
                    </div>
                     <div class="col-md-4">
                        <div class="text-center facts-data-box bg_tweets_lightblue">
                            <div class="inner-div">
                                <span class="image-wrap">
                                    <img src="img/white-bulb.png" class="image_top ">
                                </span>
                                <div class="text-center twit-all-content facts-content">save<span class="number-font"> 66% </span>on LED purchase</div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="text-center facts-data-box bg_facts_grey">
                            <div class="inner-div">
                                <span class="image-wrap">
                                    <img src="img/watch.png" class="image_top ">
                                </span>
                                <div class="text-center twit-all-content facts-content_blue">
                                    LEDs can be dimmed allowing the amount of light to be controlled.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
               <div id="loadingDiv"><img src="img/362.GIF"></div>
            </div>
            <div class="row">
                <div class="col-lg-12 col-sm-12" >
                    <div class="load-more">
                        <a id="facts-more" href="javascript:void(0);">load more</a>
                    </div>
                </div>
            </div>

    <script>
    $(function(){
        $('#facts-more').on( "click", function() {
            $('#loadingDiv').fadeIn(100);
            $.ajax({
                url: "ajax/facts.html",
                success: function (data) { 
                    $('#loadingDiv').fadeOut(500);
                    $('.facts-show-more').append(data);
                     },
                dataType: 'html'
                

            });
  
        });

       


    });
</script>