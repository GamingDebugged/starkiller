</div><!--main-content-wrap-->
			<footer id="foot-wrap" class="left relative">
				<div class="sec-marg-out relative">
					<div class="sec-marg-in">
						<div id="foot-top" class="left relative">
							<?php $footer_info = get_option('mvp_footer_info'); if ($footer_info == "true") { ?>
								<div class="foot-widget left relative">
									<?php if(get_option('mvp_logo_footer')) { ?>
										<div class="foot-logo left relative" itemscope itemtype="http://schema.org/Organization">
											<img itemprop="logo" src="<?php echo esc_url(get_option('mvp_logo_footer')); ?>" alt="<?php bloginfo( 'name' ); ?>" width="200" height="60" loading="lazy" />
										</div><!--foot-logo-->
									<?php } else { ?>
										<div class="foot-logo left relative" itemscope itemtype="http://schema.org/Organization">
											<img itemprop="logo" src="<?php echo get_template_directory_uri(); ?>/images/logos/logo-foot.png" alt="<?php bloginfo( 'name' ); ?>" width="200" height="60" loading="lazy" />
										</div><!--foot-logo-->
									<?php } ?>
									<div class="foot-info-text left relative">
										<?php echo wp_kses_post(get_option('mvp_footer_text')); ?>
									</div><!--footer-info-text-->
									<div class="foot-soc left relative">
										<ul class="foot-soc-list relative">
											<?php if(get_option('mvp_facebook')) { ?>
												<li class="foot-soc-fb">
													<a href="<?php echo esc_url(get_option('mvp_facebook')); ?>" aria-label="Follow us on Facebook" target="_blank" rel="noopener noreferrer"><i class="fa fa-facebook-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_twitter')) { ?>
												<li class="foot-soc-twit">
													<a href="<?php echo esc_url(get_option('mvp_twitter')); ?>" aria-label="Follow us on Twitter" target="_blank" rel="noopener noreferrer"><i class="fa fa-twitter-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_pinterest')) { ?>
												<li class="foot-soc-pin">
													<a href="<?php echo esc_url(get_option('mvp_pinterest')); ?>" aria-label="Follow us on Pinterest" target="_blank" rel="noopener noreferrer"><i class="fa fa-pinterest-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_instagram')) { ?>
												<li class="foot-soc-inst">
													<a href="<?php echo esc_url(get_option('mvp_instagram')); ?>" aria-label="Follow us on Instagram" target="_blank" rel="noopener noreferrer"><i class="fa fa-instagram fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_google')) { ?>
												<li class="foot-soc-goog">
													<a href="<?php echo esc_url(get_option('mvp_google')); ?>" aria-label="Follow us on Google Plus" target="_blank" rel="noopener noreferrer"><i class="fa fa-google-plus-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_youtube')) { ?>
												<li class="foot-soc-yt">
													<a href="<?php echo esc_url(get_option('mvp_youtube')); ?>" aria-label="Subscribe to our YouTube channel" target="_blank" rel="noopener noreferrer"><i class="fa fa-youtube-play fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_linkedin')) { ?>
												<li class="foot-soc-link">
													<a href="<?php echo esc_url(get_option('mvp_linkedin')); ?>" aria-label="Connect on LinkedIn" target="_blank" rel="noopener noreferrer"><i class="fa fa-linkedin-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_tumblr')) { ?>
												<li class="foot-soc-tumb">
													<a href="<?php echo esc_url(get_option('mvp_tumblr')); ?>" aria-label="Follow us on Tumblr" target="_blank" rel="noopener noreferrer"><i class="fa fa-tumblr-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_tiktok')) { ?>
												<li class="foot-soc-tiktok">
													<a href="<?php echo esc_url(get_option('mvp_tiktok')); ?>" aria-label="Follow us on TikTok" target="_blank" rel="noopener noreferrer"><i class="fa fa-music fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_discord')) { ?>
												<li class="foot-soc-discord">
													<a href="<?php echo esc_url(get_option('mvp_discord')); ?>" aria-label="Join our Discord server" target="_blank" rel="noopener noreferrer"><i class="fa fa-comments fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_twitch')) { ?>
												<li class="foot-soc-twitch">
													<a href="<?php echo esc_url(get_option('mvp_twitch')); ?>" aria-label="Watch us on Twitch" target="_blank" rel="noopener noreferrer"><i class="fa fa-twitch fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
											<?php if(get_option('mvp_rss')) { ?>
												<li class="foot-soc-rss">
													<a href="<?php echo esc_url(get_option('mvp_rss')); ?>" aria-label="Subscribe to RSS feed" target="_blank" rel="noopener noreferrer"><i class="fa fa-rss-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } else { ?>
												<li class="foot-soc-rss">
													<a href="<?php bloginfo('rss2_url'); ?>" aria-label="Subscribe to RSS feed" target="_blank" rel="noopener noreferrer"><i class="fa fa-rss-square fa-2" aria-hidden="true"></i></a>
												</li>
											<?php } ?>
										</ul>
									</div><!--foot-soc-->
								</div><!--foot-widget-->
							<?php } ?>
							<?php if (!function_exists('dynamic_sidebar') || !dynamic_sidebar('Footer Widget Area')): endif; ?>
						</div><!--footer-top-->
						<div id="foot-bot" class="left relative">
							<nav class="foot-menu relative" role="navigation" aria-label="Footer menu">
								<?php wp_nav_menu(array('theme_location' => 'footer-menu', 'depth' => 1)); ?>
							</nav><!--foot-menu-->
							<div class="foot-copy relative">
								<p>&copy; <?php echo date('Y'); ?> <?php bloginfo('name'); ?>. <?php echo wp_kses_post(get_option('mvp_copyright')); ?></p>
								<p class="foot-links">
									<a href="<?php echo esc_url(home_url('/privacy-policy/')); ?>">Privacy Policy</a> | 
									<a href="<?php echo esc_url(home_url('/terms-of-service/')); ?>">Terms of Service</a> | 
									<a href="<?php echo esc_url(home_url('/contact/')); ?>">Contact</a> | 
									<a href="<?php echo esc_url(home_url('/about/')); ?>">About</a>
								</p>
							</div><!--foot-copy-->
						</div><!--foot-bot-->
					</div><!--sec-marg-in-->
				</div><!--sec-marg-out-->
			</footer>
			
			<!-- ========== MASHABLE PARTNER BAR - START ========== -->
			<!-- REMOVE THIS ENTIRE SECTION IF PAYMENT NOT RECEIVED -->
			<div id="mashable-partner-bar" class="left relative">
				<div class="mashable-bar-inner">
					<p class="mashable-text">
						<strong>Mashable is a global, multi-platform media and entertainment company</strong><br>
						For more queries and news contact us on this<br>
						Email: <a href="mailto:info@mashablepartners.com">info@mashablepartners.com</a>
					</p>
				</div>
			</div>
			<!-- ========== MASHABLE PARTNER BAR - END ========== -->
			
		</div><!--content-in-->
	</div><!--content-out-->
		</div><!--site-fixed-->
	</div><!--site-out-->
</div><!--site-->

<?php wp_footer(); ?>

</body>
</html>
