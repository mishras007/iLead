
<div class="col-lg-12">
	<div class="treding-tab">
		<ul>
			<li>
				<a href="javascript:;" class="homepg-tab active-teding-tab active" data-url="include/all-content.php">ALL</a>
			</li>
		
			<li>
				<a href="javascript:;" class="homepg-tab" data-url="include/videos-content.php">VIDEOS</a>
			</li>
			
			<li>
				<a href="javascript:;" class="homepg-tab" data-url="include/tweets-content.php">TWEETS</a>
			</li>
			
			<li>
				<a href="javascript:;" class="homepg-tab" data-url="include/facts-content.php">FACTS</a>
			</li>
		</ul>
		<div class="clearfix"></div>
		<div class="treding-tab-content">

		</div>
	</div>
</div>
<script src="js/jquery.min.js" type="text/javascript"></script>
<script>
	$(function(){
		$(".treding-tab-content").load("include/all-content.php");
		$(".homepg-tab").on("click", function(){
			var getUrl = $(this).attr("data-url");
			console.log("Testing " + getUrl);
			$(".treding-tab-content").load(getUrl);
			//$(this).addClass(".active-teding-tab");
			$(".homepg-tab").removeClass("active-teding-tab");
			$(this).addClass("active-teding-tab");
		});		
		console.log("Ok");
	});
</script>